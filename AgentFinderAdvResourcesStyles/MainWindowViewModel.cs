using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Xml.Serialization;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;

namespace AgentFinderAdvResourcesStyles
{
    class MainWindowViewModel : BindableBase
    {
        private string _availableFiletypes = "xml files (*.xml)|*.xml";

        public MainWindowViewModel()
        {
            agents = new ObservableCollection<Agent>
            {
                new Agent("007", "James Bond", "Murder", "Assinate the pope"),
                new Agent("021", "Morten Hansen", "Hacking", "Hack the Pentagon")
            };
        }

        #region Properties

        private ObservableCollection<Agent> agents;
        public ObservableCollection<Agent> Agents
        {
            get { return agents; }
            set => agents = value;
        }

        private Agent currentAgent = null;
        public Agent CurrentAgent
        {
            get { return currentAgent; }
            set { SetProperty(ref currentAgent, value); }
        }

        private string _filename = null;
        public string Filename
        {
            get => _filename;
            private set => SetProperty(ref _filename, value);
        }

        private int currentIndex = -1;
        public int CurrentIndex
        {
            get { return currentIndex; }
            set { SetProperty(ref currentIndex, value); }
        }

        private string backgroundColor = "White";

        public string BackgroundColor
        {
            get { return backgroundColor; }
            set { SetProperty(ref backgroundColor, value); }
        }

        #endregion
        

        #region Commands

       private ICommand _previousCommand;
        public ICommand PreviousCommand
        {
            get
            {
                return _previousCommand ??
                       (_previousCommand = new DelegateCommand(
                        PreviousCommandExecute, PreviousCommandCanExecute)
                        .ObservesProperty(() => CurrentIndex));
            }
        }

        private void PreviousCommandExecute()
        {
            if (CurrentIndex > 0)
                CurrentIndex--;
        }

        private bool PreviousCommandCanExecute()
        {
            return (CurrentIndex > 0);
        }

        private ICommand _nextCommand;
        public ICommand NextCommand => _nextCommand ?? (
                                           _nextCommand = new DelegateCommand(() => CurrentIndex++,
                                                   () => CurrentIndex < (Agents.Count - 1))
                                               .ObservesProperty(() => CurrentIndex));

        private ICommand _addNewCommand;
        public ICommand AddNewCommand
        {
            get
            {
                return _addNewCommand ?? (_addNewCommand = new DelegateCommand(() =>
                {
                    Agents.Add(new Agent());
                    CurrentIndex = Agents.Count - 1;
                }));
            }
        }

        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??
                       (_deleteCommand = new DelegateCommand(() => { Agents.RemoveAt(CurrentIndex); }));
            }
        }

        private ICommand _setBackgroundCommand;
        public ICommand SetBackgroundCommand
        {
            get { return _setBackgroundCommand ?? (_setBackgroundCommand = new DelegateCommand<string>(SetBackgroundCommand_Execute)); }
        }

        private void SetBackgroundCommand_Execute(string argBackgroundColor)
        {
            if (argBackgroundColor != BackgroundColor)
                BackgroundColor = argBackgroundColor;
        }

        ICommand _saveAs;
        public ICommand SaveAs
        {
            get
            {
                return _saveAs ?? (_saveAs = new DelegateCommand(() => { executeSaveAs(); }));
            }
        }

        public void executeSaveAs()
        {
            XmlSerializer XML_serial = new XmlSerializer(typeof(ObservableCollection<Agent>));

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = _availableFiletypes;
            saveFileDialog1.ShowDialog();
            Filename = saveFileDialog1.FileName;

            TextWriter writer = new StreamWriter(Filename);

            XML_serial.Serialize(writer, Agents);

            writer.Close();
        }

        ICommand _open;
        public ICommand Open
        {
            get
            {
                return _open ?? (_open = new DelegateCommand(() => { executeOpen(); }));
            }
        }

        public void executeOpen()
        {
            XmlSerializer XML_serial = new XmlSerializer(typeof(ObservableCollection<Agent>));


            OpenFileDialog OpenFileDialog1 = new OpenFileDialog();
            OpenFileDialog1.Filter = _availableFiletypes;
            OpenFileDialog1.ShowDialog();
            Filename = OpenFileDialog1.FileName;
            FileStream fs = new FileStream(Filename, FileMode.Open);
            Agents = (ObservableCollection<Agent>)XML_serial.Deserialize(fs);
            fs.Close();
        }

        ICommand _save;
        public ICommand Save
        {
            get
            {
                return _save ?? (_save = new DelegateCommand(() => { executeSave(); }));
            }
        }

        public void executeSave()
        {
            XmlSerializer XML_serial = new XmlSerializer(typeof(ObservableCollection<Agent>));
            TextWriter writer = new StreamWriter(Filename);
            XML_serial.Serialize(writer, Agents);
            writer.Close();
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new DelegateCommand(() => App.Current.MainWindow.Close())); }
        }

        #endregion
    }
}
