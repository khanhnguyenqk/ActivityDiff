using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Infrastructure.DataType;
using Infrastructure.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using XmlDocumentWrapper;

namespace XmlItemDiffTool
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private bool notRunning = true;

        public bool NotRunning
        {
            get { return notRunning; }
            set
            {
                if(value != notRunning)
                {
                    notRunning = value;
                    NotifyPropertyChanged(@"NotRunning");
                }
            }
        }

        private string filePath1 = String.Empty;

        public string FilePath1
        {
            get { return filePath1; }
            set
            {
                if(value != null && !value.Equals(filePath1))
                {
                    filePath1 = value;
                    NotifyPropertyChanged(@"FilePath1");
                }
            }
        }

        private string filePath2 = String.Empty;

        public string FilePath2
        {
            get { return filePath2; }
            set
            {
                if(value != null && !value.Equals(filePath2))
                {
                    filePath2 = value;
                    NotifyPropertyChanged(@"FilePath2");
                }
            }
        }

        private string result = String.Empty;

        public string Result
        {
            get { return result; }
            set
            {
                if(value != null && !value.Equals(result))
                {
                    result = value;
                    NotifyPropertyChanged(@"Result");
                } 
            }
        }

        public DelegateCommand CompareCommand { get; set; }

        public MainWindowViewModel()
        {
            CompareCommand = new DelegateCommand(CompareCommandExecuted);
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            unobservedTaskExceptionEventArgs.Exception.Flatten();
            foreach(var e in unobservedTaskExceptionEventArgs.Exception.InnerExceptions)
            {
                MessageBox.Show(@"Unexpected error:" + Environment.NewLine + e);
            }
            unobservedTaskExceptionEventArgs.SetObserved();
            NotRunning = true;
        }

        private void CompareCommandExecuted()
        {
            try
            {
                NotRunning = false;
                XmlDocumentConstructed doc1 = null;
                XmlDocumentConstructed doc2 = null;
                Task t = Task.Factory.StartNew(() =>
                {
                    doc1 = XmlDocumentParser.ConstructFromFile(FilePath1);
                    doc2 = XmlDocumentParser.ConstructFromFile(FilePath2);

                    XmlDocumentHistoryComparer.CreateHistoryTrace(doc1, doc2);
                });
                t.ContinueWith((antecedent) =>
                {
                    try
                    {
                        Result = XmlDocumentHistoryComparer.HistoryTraceToString(doc1, doc2);
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(@"Unexpected error:" + Environment.NewLine + e);
                    }
                    finally
                    {
                        NotRunning = true;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch(Exception e)
            {
                NotRunning = true;
                MessageBox.Show(@"Unexpected error:" + Environment.NewLine + e);
            }
        }
    }
}
