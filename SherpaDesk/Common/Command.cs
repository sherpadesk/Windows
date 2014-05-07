using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SherpaDesk.Common
{
    public sealed class Command
    {
        private string _command;

        public Command(string command)
        {
            _command = command;
        }

        public static Command operator +(Command c, string str)
        {
            c._command += str;
            return c;
        }

        public override string ToString()
        {
            return _command;
        }
    }

    public sealed class Commands
    {
        public static Commands Empty = new Commands();

        public Command Time
        {
            get
            {
                return new Command("time");
            }
        }

        public Command Tickets
        {
            get
            {
                return new Command("tickets");
            }
        }

        public Command Posts
        {
            get
            {
                return new Command("posts");
            }
        }

        public Command Files
        {
            get
            {
                return new Command("files");
            }
        }

        public Command Activity
        {
            get
            {
                return new Command("activity");
            }
        }

        public Command Accounts
        {
            get
            {
                return new Command("accounts");
            }
        }

        public Command Classes
        {
            get
            {
                return new Command("classes");
            }
        }

        public Command Users
        {
            get
            {
                return new Command("users");
            }
        }
        public Command Technicians
        {
            get
            {
                return new Command("technicians");
            }
        }

        public Command TaskTypes
        {
            get
            {
                return new Command("task_types");
            }
        }
        public Command Projects
        {
            get
            {
                return new Command("projects");
            }
        }
        public Command Login
        {
            get
            {
                return new Command("login");
            }
        }
        public Command Organizations
        {
            get
            {
                return new Command("organizations");
            }
        }
    }
}
