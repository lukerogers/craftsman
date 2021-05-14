﻿namespace Craftsman.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using static Helpers.ConsoleWriter;

    public static class ListCommand
    {
        public static void Run()
        {
            WriteHelpHeader(@$"Usage:");
            WriteHelpText(@$"   command [options] [arguments]{Environment.NewLine}");

            WriteHelpHeader(@$"Commands:");

            WriteHelpHeader(@$"    assistance");
            WriteHelpText(@$"      list                List commands");
            WriteHelpText(@$"      version, --version  Display the current Craftsman version.");
            WriteHelpText(@$"      -h, --help          Display help for a command");

            WriteHelpHeader(@$"{Environment.NewLine}    add");
            WriteHelpText(@$"      add:bc              Add an API to your DDD project.");
            WriteHelpText(@$"      add:bus             Add a message bus to a web api.");
            WriteHelpText(@$"      add:entity          Add a new entity to a web api.");
            WriteHelpText(@$"      add:prop            Add a new property to an entity in a web api.");

            WriteHelpHeader(@$"{Environment.NewLine}    new");
            WriteHelpText(@$"      new:domain          Create a new DDD based project");

            WriteHelpHeader(@$"{Environment.NewLine}Options:");
            WriteHelpText(@$"   -h, --help          Display this help message");

            WriteHelpHeader(@$"{Environment.NewLine}Example Help Options:");
            WriteHelpText(@$"   craftsman new:domain -h");
            WriteHelpText(@$"   craftsman add:bc -h");
            WriteHelpText(@$"   craftsman add:bus -h");
            WriteHelpText(@$"   craftsman add:entity -h");
            WriteHelpText(@$"   craftsman add:property -h");
        }
    }
}