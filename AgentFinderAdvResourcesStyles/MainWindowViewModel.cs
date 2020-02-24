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
using Prism.Commands;
using Prism.Mvvm;

namespace AgentFinderAdvResourcesStyles
{
    class MainWindowViewModel : BindableBase
    {
        private ObservableCollection<Agent> agents;
        private string filename = "";

        public MainWindowViewModel()
        {
            agents = new ObservableCollection<Agent>
            {
                new Agent("007", "James Bond", "Murder", "Assinate the pope"),
                new Agent("021", "Morten Hansen", "Hacking", "Hack the Pentagon")
            };
        }

        #region Properties

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

        private ICommand _newCommand;
        public ICommand NewCommand
        {
            get { return _newCommand ?? (_newCommand = new DelegateCommand(NewFileCommand_Execute)); }
        }

        public void NewFileCommand_Execute()
        {
            MessageBoxResult res = MessageBox.Show("Any unsaved data will be lost. Are you sure you want to continue?",
                "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                Agents.Clear();
                filename = "";
                Agents.Add(new Agent());
                CurrentIndex = Agents.Count - 1;
            }
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new DelegateCommand(() => App.Current.MainWindow.Close())); }
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

        ICommand _SaveAsCommand;
        public ICommand SaveAsCommand
        {
            get { return _SaveAsCommand ?? (_SaveAsCommand = new DelegateCommand<string>(SaveAsCommand_Execute)); }
        }

        private void SaveAsCommand_Execute(string argFilename)
        {
            if (argFilename == "")
            {
                MessageBox.Show("You must enter a file name in the File Name textbox!", "Unable to save file",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                filename = argFilename;
                SaveFileCommand_Execute();
            }
        }

        ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return _SaveCommand ?? (_SaveCommand = new DelegateCommand(SaveFileCommand_Execute, SaveFileCommand_CanExecute)
                           .ObservesProperty(() => Agents.Count));
            }
        }

        private void SaveFileCommand_Execute()
        {
            // Create an instance of the XmlSerializer class and specify the type of object to serialize.
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Agent>));
            TextWriter writer = new StreamWriter(filename);
            // Serialize all the agents.
            serializer.Serialize(writer, Agents);
            writer.Close();
        }

        private bool SaveFileCommand_CanExecute()
        {
            return (filename != "") && (Agents.Count > 0);
        }

        ICommand _OpenFileCommand;
        public ICommand OpenFileCommand
        {
            get { return _OpenFileCommand ?? (_OpenFileCommand = new DelegateCommand<string>(OpenFileCommand_Execute)); }
        }

        private void OpenFileCommand_Execute(string argFilename)
        {
            if (argFilename == "")
            {

                MessageBox.Show("You must enter a file name in the File Name textbox!", "Unable to save file",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                filename = argFilename;
                var tempAgents = new ObservableCollection<Agent>();

                // Create an instance of the XmlSerializer class and specify the type of object to deserialize.
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Agent>));
                try
                {
                    TextReader reader = new StreamReader(filename);
                    // Deserialize all the agents.
                    tempAgents = (ObservableCollection<Agent>)serializer.Deserialize(reader);
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unable to open file", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Agents = tempAgents;

                // We have to insert the agents in the existing collection. 
                //Agents.Clear();
                //foreach (var agent in tempAgents)
                //    Add(agent);
            }
        }

        #endregion
    }
}
