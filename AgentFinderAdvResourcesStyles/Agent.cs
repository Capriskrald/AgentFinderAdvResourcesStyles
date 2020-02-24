using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AgentFinderAdvResourcesStyles
{
    public class Agent : INotifyPropertyChanged
    {
        // INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        protected void Notify([CallerMemberName]string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        string id;
        string codeName;
        string speciality;
        string assignment;

        public Agent()
        {
        }

        public Agent(string aId, string aName, string aSpeciality, string aAssignment)
        {
            this.id = aId;
            this.codeName = aName;
            this.speciality = aSpeciality;
            this.assignment = aAssignment;
        }

        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                if (this.id.Any(x => !char.IsLetter(x)))
                    throw new ArgumentException("ID can't contain letters");
                else
                {
                    this.id = value;
                    Notify();
                }
            }
        }

        public string CodeName
        {
            get
            {
                return this.codeName;
            }
            set
            {
                this.codeName = value;
                Notify();
            }
        }

        public string Speciality
        {
            get
            {
                return this.speciality;
            }
            set
            {
                this.speciality = value;
                Notify();
            }
        }

        public string Assignment
        {
            get
            {
                return this.assignment;
            }
            set
            {
                this.assignment = value;
                Notify();
            }
        }
    }
}
