using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.DataType;
using Infrastructure.Enum;
using Infrastructure.Interface;
using Infrastructure.ObjectModel;

namespace XmlDocumentWrapper
{
    public static class XmlDocumentHistoryComparer
    {
        public static string HistoryTraceToString(XmlWorkflowItem before, XmlWorkflowItem after)
        {
            if(before.HistoryStates.Count == 0)
            {
                return String.Empty;
            }
            if(before.HistoryStates.Count == 1 && before.HistoryStates.Contains(HistoryState.I))
            {
                return String.Empty;
            }

            string ret = String.Empty;
            if(!(before.HistoryStates.Count == 1 && before.HistoryStates.Contains(HistoryState.D)))
            {
                ret = before.GetPathToRoot() + @": ";
                foreach(var historyState in before.HistoryStates)
                {
                    if(historyState != HistoryState.I &&
                       historyState != HistoryState.P &&
                       historyState != HistoryState.L &&
                       historyState != HistoryState.D)
                    {
                        ret += historyState.ToString();   
                    }
                }
                ret += Environment.NewLine;
                if(before.HistoryStates.Contains(HistoryState.L))
                {
                    ret += @"    Moved to a different level -> ";
                    ret += after.GetNode(before.Id).GetPathToRoot() + Environment.NewLine;
                }
                if(before.HistoryStates.Contains(HistoryState.P))
                {
                    ret += @"    Properties changed:" + Environment.NewLine;
                    foreach(var item in before.ChangedProperties)
                    {
                        ret += @"        " + item.OriginalProperty.GetPathToRoot() + @" "
                               + item + Environment.NewLine;
                    }
                }
            }

            foreach(var xmlWorkflowItem in before.Children)
            {
                ret += HistoryTraceToString(xmlWorkflowItem, after);
            }

            return ret;
        }

        public static string AddedWorkflowItemsDemonstration(XmlWorkflowItem after)
        {
            string ret = String.Empty;
            if(after.HistoryStates.Contains(HistoryState.A))
            {
                ret = after.GetPathToRoot() + @": A" + Environment.NewLine;
            }
            foreach(var xmlWorkflowItem in after.Children)
            {
                ret += AddedWorkflowItemsDemonstration(xmlWorkflowItem);
            }

            return ret;
        }

        public static void CreateHistoryTrace(XmlWorkflowItem before, XmlWorkflowItem after)
        {
            if(before.Equals(after))
            {
                before.HistoryStates.Add(HistoryState.I);
                after.HistoryStates.Add(HistoryState.I);
                return;
            }

            before.HistoryStates.Add(HistoryState.D);
            after.HistoryStates.Add(HistoryState.D);

            if(!before.ArePropertiesEqual(after) || !before.Expressions.Equals(after.Expressions))
            {
                before.HistoryStates.Add(HistoryState.P);
                after.HistoryStates.Add(HistoryState.P);

                List<XmlPropertyHistory> l = CreatePropertyChangeHistory(before, after);
                AddItems(before.ChangedProperties, l);
            }

            List<XmlWorkflowItem> beforeClone = new List<XmlWorkflowItem>();
            foreach(var child in before.Children)
            {
                beforeClone.Add(child);
            }

            List<XmlWorkflowItem> afterClone = new List<XmlWorkflowItem>();
            foreach(var child in after.Children)
            {
                afterClone.Add(child);
            }

            List<XmlWorkflowItem> removeListFromBefore = new List<XmlWorkflowItem>();
            List<XmlWorkflowItem> removeListFromAfter = new List<XmlWorkflowItem>();

            // Remove any entry that have L mark
            foreach(var item in beforeClone)
            {
                if(item.HistoryStates.Contains(HistoryState.L))
                {
                    removeListFromBefore.Add(item);
                }
            }
            foreach(var item in afterClone)
            {
                if(item.HistoryStates.Contains(HistoryState.L))
                {
                    removeListFromAfter.Add(item);
                }
            }
            RemoveItems(beforeClone, removeListFromBefore);
            removeListFromBefore.Clear();
            RemoveItems(afterClone, removeListFromAfter);
            removeListFromAfter.Clear();

            // Look for Removed
            foreach(var item in beforeClone)
            {
                if(!afterClone.ContainsItem(item.Id))
                {
                    removeListFromBefore.Add(item);
                    XmlWorkflowItem match = after.GetNode(item.Id);
                    if(match != null)
                    {
                        item.HistoryStates.Add(HistoryState.L);
                        match.HistoryStates.Add(HistoryState.L);
                        // Todo: multitask -> Can't. L state will have race condition
                        CreateHistoryTrace(item, match);
                    }
                    else
                    {
                        item.HistoryStates.Add(HistoryState.R);
                    }
                }
            }
            RemoveItems(beforeClone, removeListFromBefore);
            removeListFromBefore.Clear();

            // Look for Added
            foreach(var item in afterClone)
            {
                if(!beforeClone.ContainsItem(item.Id))
                {
                    removeListFromAfter.Add(item);
                    XmlWorkflowItem match = before.GetNode(item.Id);
                    if(match != null)
                    {
                        item.HistoryStates.Add(HistoryState.L);
                        match.HistoryStates.Add(HistoryState.L);
                        // Todo: multitask -> Can't. L state will have race condition
                        CreateHistoryTrace(match, item);
                    }
                    else
                    {
                        item.HistoryStates.Add(HistoryState.A);
                    }
                }
            }
            RemoveItems(afterClone, removeListFromAfter);
            removeListFromAfter.Clear();

            CheckItemMovedAround(beforeClone, afterClone);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="before"></param>
        /// <param name="after"></param>
        public static List<XmlPropertyHistory> CreatePropertyChangeHistory(XmlType before, XmlType after)
        {
            if(!before.TypeName.Equals(after.TypeName)) // Not gonna compare 2 different types. No thank you.
            {
                // Throw exception to let you know you made a dumb call
                throw new TypeMismatchException(@"Can't compare 2 different types.");
            }

            List<XmlPropertyHistory> ret = new List<XmlPropertyHistory>();

            foreach(var property in before.Properties)  // Traverse through properties
            {
                if(!after.Properties.Contains(property))
                {

                    XmlPropertyAbstract match = (from p in after.Properties
                                                 where p.Name.Equals(property.Name)
                                                 select p).FirstOrDefault();
                    // Todo: I don't want to deal with this now. So throw exception
                    if(match == null)
                    {
                        throw new NotImplementedException(@"Cannot deal with property disappeared yet.");
                    }
                    if(property.GetType() != match.GetType())   // NULL property case
                    {
                        if(property is XmlStringProperty && match is XmlTypeProperty)
                        {
                            XmlStringProperty mStringProperty = property as XmlStringProperty;
                            if(mStringProperty.Value.ToString().ToLower().Contains("null"))
                            {
                                XmlPropertyHistory sph = new XmlPropertyHistory
                                {
                                    OriginalProperty = property,
                                    OriginalValue = null,
                                    ChangedValue = match.Value
                                };
                                ret.Add(sph);
                            }
                            else
                            {
                                throw new NotImplementedException(@"Cannot deal with this case yet.");
                            }
                        }
                        else if(property is XmlTypeProperty && match is XmlStringProperty)
                        {
                            XmlStringProperty mStringProperty = match as XmlStringProperty;
                            if(mStringProperty.Value.ToString().ToLower().Contains("null"))
                            {
                                XmlPropertyHistory sph = new XmlPropertyHistory
                                {
                                    OriginalProperty = property,
                                    OriginalValue = property.Value,
                                    ChangedValue = null
                                };
                                ret.Add(sph);
                            }
                            else
                            {
                                throw new NotImplementedException(@"Cannot deal with this case yet.");
                            }
                        }
                        else
                        {
                            throw new NotImplementedException(@"Cannot deal with this case yet.");
                        }
                    }
                    else if(property is XmlStringProperty)
                    {
                        XmlStringProperty mStringProperty = match as XmlStringProperty;
                        if(mStringProperty != null)
                        {
                            XmlPropertyHistory sph = new XmlPropertyHistory
                            {
                                OriginalProperty = property,
                                OriginalValue = (property as XmlStringProperty).Value.ToString(),
                                ChangedValue = mStringProperty.Value.ToString()
                            };
                            ret.Add(sph);
                        }
                    }
                    else if(property is XmlTypeProperty)
                    {
                        XmlTypeProperty tMatch = match as XmlTypeProperty;
                        if(tMatch != null)
                        {
                            var pValue = property.Value as XmlType;
                            var mValue = tMatch.Value as XmlType;
                            if(mValue != null && pValue != null)
                            {
                                if(pValue.TypeName.Equals(mValue.TypeName))
                                {
                                    List<XmlPropertyHistory> result =
                                        CreatePropertyChangeHistory(property.Value as XmlType, tMatch.Value as XmlType);
                                    AddItems(ret, result);
                                }
                                else
                                {
                                    XmlPropertyHistory ph = new XmlPropertyHistory
                                    {
                                        OriginalProperty = property,
                                        OriginalValue = pValue.TypeName,
                                        ChangedValue = mValue.TypeName
                                    };
                                    ret.Add(ph);
                                }
                            }
                        }
                    }
                }
            }

            if(before is XmlDataItem && after is XmlDataItem)       // Array case
            {
                XmlDataItem dBefore = before as XmlDataItem;
                XmlDataItem dAfter = after as XmlDataItem;

                if(dBefore.Children.Count != dAfter.Children.Count)
                {
                    XmlPropertyHistory ph = new XmlPropertyHistory
                    {
                        OriginalProperty = dBefore.PropertyHost,
                        OriginalValue = dBefore.Children.Count,
                        ChangedValue = dAfter.Children.Count
                    };
                    ret.Add(ph);
                }

                int count = Math.Min(dBefore.Children.Count, dAfter.Children.Count);
                for(int i = 0; i < count; i++)
                {
                    List<XmlPropertyHistory> result =
                                CreatePropertyChangeHistory(dBefore.Children[i], dAfter.Children[i]);
                    AddItems(ret, result);
                }
            }

            return ret;
        }

        /// <summary>
        /// 2 sets of identical workflow items (Id wise identical). Determine if any item was moved out of order
        /// </summary>
        /// <param name="before"></param>
        /// <param name="after"></param>
        private static void CheckItemMovedAround(IList<XmlWorkflowItem> before, IList<XmlWorkflowItem> after)
        {
            if(before.Count != after.Count)
            {
                throw new Exception(@"Can't perform on 2 different set.");
            }

            if(before.Count == 0)
            {
                return;
            }

            if(before[0].Id.Equals(after[0].Id))    // Item is in place.
            {
                CreateHistoryTrace(before[0], after[0]);
                // Remove item from the list and check the other ones still in the set
                before.Remove(before[0]);
                after.Remove(after[0]);
                CheckItemMovedAround(before, after);
            }
            else
            {   // Item not in place, mark M, remove and call recursively
                XmlWorkflowItem match = after.GetNode(before[0].Id);
                if(match == null)
                {
                    throw new Exception(@"Can't perform on 2 different set.");
                }
                CreateHistoryTrace(before[0], match);
                before[0].HistoryStates.Add(HistoryState.M);
                match.HistoryStates.Add(HistoryState.M);
                before.Remove(before[0]);
                after.Remove(match);
                CheckItemMovedAround(before, after);
            }
        }

        private static XmlWorkflowItem GetNode(this IEnumerable<XmlWorkflowItem> list, string id)
        {
            foreach(var item in list)
            {
                if(item.Id.Equals(id))
                {
                    return item;
                }
            }
            return null;
        }

        private static void RemoveItems(IList from, IList at)
        {
            foreach(var item in at)
            {
                from.Remove(item);
            }
        }

        /// <summary>
        /// Add items from a list to the end of another list.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        private static void AddItems(IList to, IList from)
        {
            if(from == null)
            {
                return;
            }
            if(to == null)
            {
                to = from;
                return;
            }
            foreach(var item in from)
            {
                to.Add(item);
            }
        }

        private static bool ContainsItem(this IEnumerable<XmlWorkflowItem> list, string id)
        {
            foreach(var item in list)
            {
                if(item.Id.Equals(id))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
