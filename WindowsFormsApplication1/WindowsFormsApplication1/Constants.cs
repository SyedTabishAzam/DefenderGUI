using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public static class Constants
    {
        public static class Filename
        {

            public const string SCNERIO_DATA_FILE = "ScenerioData.dat";
            public const string SUBSCRIBER_FILE = "InitialSubscriber.exe";
            public const string PUBLISEHR_FILE = "CommandPublisher.exe";
            public const string ENV_VAR_FILE = "SetupEnv.bat";
            public const string RTPS_FILE = "rtps.ini";
            public const string COMMAND_FILE = "commands.dat";

        }

        public static class Status
        {
            public const string SUB_START = "Starting Subscriber";
            public const string CHECKING_PROCESS = "Loading Subscriber";
            public const string LISTENING = "Listening to scenerio data";
            public const string LOAD_COMPLETE = "Load Complete";
            public const string LOAD_FAILED = "File not found, Try again!";
            public const string VALIDATION_FAILED = "Can't read file structure. Try again!";
            public const string RESTART_APP = "This will restart the application and load everything again. Are you sure you want to do this?";
        }

        public static class Error
        {
            public const string LOAD_FAILED = "Load failed! Try again?";
            public const string MISSING_SUBSCRIBER = "Fatal error! subscriber missing";
            public const string FAILED_SUBSCRIBER_START = "Error! subscriber not started (ensure INI or IOR file is present)";
            public const string TYPE_MISMATCH = " Error! Value type mismatch.";
            public const string ENITITY_EMPTY = " Error! Entity name cannot be empty.";
            public const string VALUE_EMPTY = " Error! Value cannot be empty.";
            public const string VARIABLE_EMPTY = " Error! Variable cannot be empty.";
            public const string UNDEFINED_TYPE = " Error! Undefined type.";
        }

        public static class AllowedVariables
        {
            public const string BOOL = "type_bool";
            public const string STRING = "type_string";
            public const string DOUBLE = "type_double";
            public const string INT = "type_int";
            public const string FLOAT = "type_float";

            public static List<string> GetList()
            {
                List<string> ls = new List<string>();

                ls.Add(BOOL);
                ls.Add(STRING);
                ls.Add(INT);
                ls.Add(DOUBLE);
                ls.Add(FLOAT);
                return ls;
            }

        }


    }
}
