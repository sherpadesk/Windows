namespace SherpaDesk.Common
{
    public sealed class Command
    {
        private string _command;

        public Command(string command)
        {
            _command = command;
        }

        public static Command operator +(Command cmd, string str)
        {
            if (cmd != null && !string.IsNullOrEmpty(str))
            {
                cmd._command += str;
            }
            return cmd;

        }

        public override string ToString()
        {
            return _command;
        }
    }
}