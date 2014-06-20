using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using Infrastructure.DataType;
using Infrastructure.Enum;

namespace XmlDocumentWrapper
{
    public static class XmlDocumentHistoryComparer
    {
        private static readonly Tab tab = new Tab();

        public static void CreateHistoryTrace(XmlDocumentConstructed before, XmlDocumentConstructed after)
        {
            CreateHistoryTrace(before.Root, after.Root);
            CreateResourceHistory(before.Resources, after.Resources);
        }

        public static string HistoryTraceToString(XmlDocumentConstructed before, XmlDocumentConstructed after)
        {
            return HistoryTraceToString(before.Root, after.Root) + Environment.NewLine +
            AddedWorkflowItemsDemonstration(after.Root) + Environment.NewLine +
            ResourcesTraceToString(before.Resources);
        }

        public static string ResourcesTraceToString(XmlResources resources)
        {
            string ret = String.Empty;
            if(resources.AddedResources.Count > 0)
            {
                ret += @"Added resources:" + Environment.NewLine;
                foreach(var item in resources.AddedResources)
                {
                    ret += tab + item.Name + Environment.NewLine;
                }
            }

            if(resources.RemovedResources.Count > 0)
            {
                ret += @"Removed resources:" + Environment.NewLine;
                foreach(var item in resources.RemovedResources)
                {
                    ret += tab + item.Name + Environment.NewLine;
                }
            }

            if(resources.ModifiedResources.Count > 0)
            {
                ret += @"Modified resources:" + Environment.NewLine;
                foreach(XmlResource item in resources.ModifiedResources)
                {
                    if(item is XmlWorkflowTemplateResource)
                    {
                        XmlWorkflowTemplateResource wtItem = (XmlWorkflowTemplateResource)item;
                        if(wtItem.ReferencedTemplate != null)
                        {
                            ret += tab + item.Name + Environment.NewLine;
                            ret += tab + @"~~~~~~START WFTemplate~~~~~~" + Environment.NewLine;
                            string template = HistoryTraceToString(wtItem.Template, wtItem.ReferencedTemplate);
                            // Modify the string for pretty print
                            template = template.Replace(Environment.NewLine, Environment.NewLine + 3 * tab);
                            template = template.TrimEnd(' ');
                            template = 3 * tab + template;

                            ret += template;
                            ret += tab + @"~~~~~~END   WFTemplate~~~~~~" + Environment.NewLine;
                        }
                    }
                    else
                    {
                        ret += tab + item.Name + Environment.NewLine;
                        foreach(XmlPropertyHistory property in item.ChangedProperties)
                        {
                            ret += 2 * tab + property.OriginalProperty.GetResourcePath() + @" "
                                       + property + Environment.NewLine;
                        }
                    }
                }
            }

            return ret;
        }

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
                    ret += tab + @"Moved to a different level -> ";
                    ret += after.GetNode(before.Id).GetPathToRoot() + Environment.NewLine;
                }
                if(before.HistoryStates.Contains(HistoryState.P))
                {
                    if(before.NewProperties.Count > 0)
                    {
                        ret += tab + @"New Properties:" + Environment.NewLine;
                        foreach(var item in before.NewProperties)
                        {
                            ret += 2 * tab + item + Environment.NewLine;
                        }
                    }
                    else if(before.RemovedProperties.Count > 0)
                    {
                        ret += tab + @"Removed Properties:" + Environment.NewLine;
                        foreach(var item in before.RemovedProperties)
                        {
                            ret += 2 * tab + item + Environment.NewLine;
                        }
                    }
                    else if(before.ChangedProperties.Count > 0)
                    {
                        ret += tab + @"Properties changed:" + Environment.NewLine;
                        foreach(var item in before.ChangedProperties)
                        {
                            ret += 2 * tab + item.OriginalProperty.GetPathToRoot() + @" "
                                   + item + Environment.NewLine;
                        }
                    }
                    else if(before.ExpressionsHistory.HasChanges)
                    {
                        ret += tab + @"Expressions changes:" + Environment.NewLine;
                        if(before.ExpressionsHistory.AddedExpressions.Count > 0)
                        {
                            ret += 2 * tab + @"Added Expressions:" + Environment.NewLine;
                            foreach(XmlPropertyExpression expression in before.ExpressionsHistory.AddedExpressions)
                            {
                                ret += 3 * tab + expression + Environment.NewLine;
                            }
                        }

                        if(before.ExpressionsHistory.RemovedExpressions.Count > 0)
                        {
                            ret += 2 * tab + @"Removed Expressions:" + Environment.NewLine;
                            foreach(XmlPropertyExpression expression in before.ExpressionsHistory.RemovedExpressions)
                            {
                                ret += 3 * tab + expression + Environment.NewLine;
                            }
                        }

                        if(before.ExpressionsHistory.ModifiedExpressions.Count > 0)
                        {
                            ret += 2 * tab + @"Modified Expressions:" + Environment.NewLine;
                            foreach(Tuple<XmlPropertyExpression, string> tuple in before.ExpressionsHistory.ModifiedExpressions)
                            {
                                ret += 3 * tab + tuple.Item1 + String.Format(@" -> ""{0}""", tuple.Item2) + Environment.NewLine;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(@"Workflow item was marked P but there is no changes.");
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

            bool marked = false;
            if(!before.ArePropertiesEqual(after))
            {
                before.HistoryStates.Add(HistoryState.P);
                after.HistoryStates.Add(HistoryState.P);

                marked = true;

                List<XmlPropertyHistory> l = CreatePropertyChangeHistory(before, after);
                AddItems(before.ChangedProperties, l);
            }
            if(!before.Expressions.Equals(after.Expressions))
            {
                if(!marked)
                {
                    before.HistoryStates.Add(HistoryState.P);
                    after.HistoryStates.Add(HistoryState.P);
                }

                CreateExpressionHistory(before, after);
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

            // Check for added/removed properties. Removed is checked down there so check added here.
            foreach(var property in after.Properties)
            {
                XmlPropertyAbstract match = (from p in before.Properties
                                             where p.Name.Equals(property.Name)
                                             select p).FirstOrDefault();
                if(match == null)
                {
                    before.NewProperties.Add(property);
                }
            }

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
                        // Happens with namespace
                        before.RemovedProperties.Add(property);
                    }
                    else
                    {
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

            if(before is XmlResourceItem && after is XmlResourceItem)
            {
                XmlResourceItem rBefore = (XmlResourceItem)before;
                XmlResourceItem rAfter = (XmlResourceItem)after;
                foreach(XmlResourceItem item in rBefore.Children)
                {
                    List<XmlResourceItem> matches = (from i in rAfter.Children
                                                     where i.TypeName.Equals(item.TypeName)
                                                     select i).ToList();

                    if(matches.Count == 1)
                    {

                        XmlResourceItem match = matches.First();

                        List<XmlPropertyHistory> result = CreatePropertyChangeHistory(item, match);
                        AddItems(ret, result);
                    }
                    else
                    {
                        // Todo: NotImplementedException(@"Cannot deal with this case yet.");
                    }
                }
            }

            if(before is XmlMatchImageItem && after is XmlMatchImageItem)
            {
                XmlMatchImageItem rBefore = (XmlMatchImageItem)before;
                XmlMatchImageItem rAfter = (XmlMatchImageItem)after;

                if(rBefore.IsArray && rAfter.IsArray)
                {
                    if(rBefore.Children.Count != rAfter.Children.Count)
                    {
                        throw new NotImplementedException(@"Match Image resource's array has different counts.");
                    }

                    for(int i = 0; i < rBefore.Children.Count; i++)
                    {
                        List<XmlPropertyHistory> result = CreatePropertyChangeHistory(rBefore.Children[i], rAfter.Children[i]);
                        AddItems(ret, result);
                    }
                }
                else if(!rBefore.IsArray && !rAfter.IsArray)
                {
                    foreach(XmlMatchImageItem item in rBefore.Children)
                    {
                        List<XmlMatchImageItem> matches = (from i in rAfter.Children
                                                           where i.TypeName.Equals(item.TypeName)
                                                           select i).ToList();

                        if(matches.Count == 1)
                        {

                            XmlMatchImageItem match = matches.First();

                            List<XmlPropertyHistory> result = CreatePropertyChangeHistory(item, match);
                            AddItems(ret, result);
                        }
                        else
                        {
                            // Todo: NotImplementedException(@"Cannot deal with this case yet.");
                        }
                    }
                }
                else
                {
                    throw new Exception(@"Parsed xml files have different types for same properties' name.");
                }
            }

            return ret;
        }

        public static void CreateExpressionHistory(XmlWorkflowItem before, XmlWorkflowItem after)
        {
            if(!before.Expressions.Equals(after.Expressions))
            {
                // Look for removed expressions
                foreach(XmlPropertyExpression expression in before.Expressions)
                {
                    List<XmlPropertyExpression> matches = (from e in after.Expressions
                                                           where e.PropertyName.Equals(expression.PropertyName)
                                                           select e).ToList();

                    if(matches.Count == 0)
                    {
                        before.ExpressionsHistory.RemovedExpressions.Add(expression);
                    }
                    else if(matches.Count == 1) // Look for modified expressions
                    {
                        XmlPropertyExpression match = matches[0];
                        if(!expression.Expression.Equals(match.Expression))
                        {
                            before.ExpressionsHistory.ModifiedExpressions.Add(new Tuple<XmlPropertyExpression, string>(expression, match.Expression));
                        }
                    }
                    else
                    {
                        throw new Exception(@"One property has more than 1 expression.");
                    }
                }
                // Look for new expressions
                foreach(XmlPropertyExpression expression in after.Expressions)
                {
                    List<XmlPropertyExpression> matches = (from e in before.Expressions
                                                           where e.PropertyName.Equals(expression.PropertyName)
                                                           select e).ToList();

                    if(matches.Count == 0)
                    {
                        before.ExpressionsHistory.AddedExpressions.Add(expression);
                    }
                    else if(matches.Count > 1)
                    {
                        throw new Exception(@"One property has more than 1 expression.");
                    }
                }
            }
        }

        public static void CreateResourceHistory(XmlResources before, XmlResources after)
        {
            foreach(XmlResource item in before.Resources)
            {
                XmlResource match = (from r in after.Resources
                                     where r.Name.Equals(item.Name)
                                     select r).FirstOrDefault();

                if(match == null)   // Look for deleted resources
                {
                    before.RemovedResources.Add(item);
                }
                else
                {   // Look for changed resources
                    if(item.GetType() != match.GetType())
                    {
                        if(item.GetType() == typeof(XmlResource))
                        {
                            before.AddedResources.Add(item);
                        }
                        else if(match.GetType() == typeof(XmlResource))
                        {
                            before.RemovedResources.Add(item);
                        }
                        else
                        {
                            throw new NotImplementedException(@"Can't handle resource change type yet.");
                        }
                    }
                    else if(item is XmlSampleMapResource)
                    {
                        XmlSampleMapResource smBefore = (XmlSampleMapResource)item;
                        XmlSampleMapResource smAfter = (XmlSampleMapResource)match;
                        if(!smBefore.Equals(smAfter))
                        {
                            before.ModifiedResources.Add(item);

                            List<XmlPropertyHistory> result = CreatePropertyChangeHistory(
                               smBefore.SampleMap, smAfter.SampleMap);
                            AddItems(item.ChangedProperties, result);
                        }
                    }
                    else if(item is XmlTypeResource)
                    {
                        XmlTypeResource trBefore = (XmlTypeResource)item;
                        XmlTypeResource trAfter = (XmlTypeResource)match;

                        if(!trBefore.Equals(trAfter))
                        {
                            before.ModifiedResources.Add(item);

                            List<XmlPropertyHistory> result = CreatePropertyChangeHistory(
                                trBefore.Data, trAfter.Data);
                            AddItems(item.ChangedProperties, result);
                        }
                    }
                    else if(item is XmlPatMaxResource)
                    {
                        XmlPatMaxResource prBefore = (XmlPatMaxResource)item;
                        XmlPatMaxResource prAfter = (XmlPatMaxResource)match;

                        if(!prBefore.Equals(prAfter))
                        {
                            before.ModifiedResources.Add(item);

                            List<XmlPropertyHistory> result = CreatePropertyChangeHistory(
                                prBefore.Data, prAfter.Data);
                            AddItems(item.ChangedProperties, result);
                        }
                    }
                    else if(item is XmlMatchImageResource)
                    {
                        XmlMatchImageResource mrBefore = (XmlMatchImageResource)item;
                        XmlMatchImageResource mrAfter = (XmlMatchImageResource)match;

                        if(!mrBefore.Equals(mrAfter))
                        {
                            before.ModifiedResources.Add(item);

                            List<XmlPropertyHistory> result = CreatePropertyChangeHistory(
                                mrBefore.Data, mrAfter.Data);
                            AddItems(item.ChangedProperties, result);
                        }
                    }
                    else if(item is XmlSiteListResource)
                    {
                        XmlSiteListResource slBefore = (XmlSiteListResource)item;
                        XmlSiteListResource slAfter = (XmlSiteListResource)match;

                        if(!slBefore.Equals(slAfter))
                        {
                            before.ModifiedResources.Add(item);

                            // Todo: handle this
                        }
                    }
                    else if(item is XmlWorkflowTemplateResource)
                    {
                        XmlWorkflowTemplateResource wtBefore = (XmlWorkflowTemplateResource)item;
                        XmlWorkflowTemplateResource wtAfter = (XmlWorkflowTemplateResource)match;

                        if(!wtBefore.Equals(wtAfter))
                        {
                            before.ModifiedResources.Add(item);

                            CreateHistoryTrace(wtBefore.Template, wtAfter.Template);
                            wtBefore.ReferencedTemplate = wtAfter.Template;
                        }
                    }
                }
            }

            foreach(XmlResource item in after.Resources)     // Look for new resources
            {
                XmlResource match = (from r in before.Resources
                                     where r.Name.Equals(item.Name)
                                     select r).FirstOrDefault();

                if(match == null)
                {
                    before.AddedResources.Add(item);
                }
            }
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

    public class Tab
    {
        public string TabString { get; set; }

        public Tab()
        {
            TabString = "    ";
        }

        public override string ToString()
        {
            return TabString;
        }

        public static string operator *(int times, Tab t)
        {
            if(times > 0)
            {
                string ret = String.Empty;
                for(int i = 0; i < times; i++)
                {
                    ret += t.ToString();
                }

                return ret;
            }
            return "";
        }

        public static string operator *(Tab t, int times)
        {
            if(times > 0)
            {
                string ret = String.Empty;
                for(int i = 0; i < times; i++)
                {
                    ret += t.ToString();
                }

                return ret;
            }
            return "";
        }
    }
}
