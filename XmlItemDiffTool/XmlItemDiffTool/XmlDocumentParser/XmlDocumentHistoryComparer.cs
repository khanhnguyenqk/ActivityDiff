using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Enum;
using Infrastructure.Interface;
using Infrastructure.ObjectModel;

namespace XmlDocumentWrapper
{
    public static class XmlDocumentHistoryComparer
    {
        public static void CreateHistoryTrace(IHistoryTraceTree before, IHistoryTraceTree after)
        {
            if(before.Equals(after))
            {
                before.HistoryStates.Add(HistoryState.I);
                after.HistoryStates.Add(HistoryState.I);
                return;
            }

            before.HistoryStates.Add(HistoryState.D);
            after.HistoryStates.Add(HistoryState.D);

            List<IHistoryTraceTree> beforeClone = new List<IHistoryTraceTree>();
            foreach(var child in before.Children)
            {
                beforeClone.Add(child);
            }

            List<IHistoryTraceTree> afterClone = new List<IHistoryTraceTree>();
            foreach(var child in after.Children)
            {
                afterClone.Add(child);
            }

            List<IHistoryTraceTree> removeListFromBefore = new List<IHistoryTraceTree>();
            List<IHistoryTraceTree> removeListFromAfter = new List<IHistoryTraceTree>();

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
                    IHistoryTraceTree match = after.GetNode(item.Id);
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
                    IHistoryTraceTree match = before.GetNode(item.Id);
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

        private static void CheckItemMovedAround(IList<IHistoryTraceTree> before, IList<IHistoryTraceTree> after)
        {
            if(before.Count != after.Count)
            {
                throw new Exception(@"Can't perform on 2 different set.");
            }

            if(before.Count == 0)
            {
                return;
            }

            if(before[0].Id.Equals(after[0].Id))
            {
                CreateHistoryTrace(before[0], after[0]);
                before.Remove(before[0]);
                after.Remove(after[0]);
                CheckItemMovedAround(after, before);
            }
            else
            {
                IHistoryTraceTree match = after.GetNode(before[0].Id);
                if(match == null)
                {
                    throw new Exception(@"Can't perform on 2 different set.");
                }
                CreateHistoryTrace(before[0], match);
                before[0].HistoryStates.Add(HistoryState.M);
                match.HistoryStates.Add(HistoryState.M);
                before.Remove(before[0]);
                after.Remove(match);
                CheckItemMovedAround(after, before);
            }
        }

        private static IHistoryTraceTree GetNode(this IEnumerable<IHistoryTraceTree> list, string id)
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

        private static bool ContainsItem(this IEnumerable<IHistoryTraceTree> list, string id)
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
