using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Light
{
    public class CharGenContext : PerPlayerScriptContext
    {
        protected override async Task script(Player p, BufferBlock<string> input)
        {
            // todo: what happens if the player quits during chargen? do we get an exception?

            // todo: ignore input when not expecting it

            p.Tell(@"Welcome to Chargen.
            Your GUID is {0}.", p.Guid);

            var name = "";

            while (name == "")
            {
                p.Tell("\nWhat is your name? >");

                string s = (await input.ReceiveAsync()).Trim();
                Action<string> e = (m => p.Tell(m, "'" + s + "'"));
                
                // TODO: check if name is already in use and the user would like to disambiguate?

                // One-character names are allowed if they're not some English alphabet character, basically.
                // Otherwise, names must be at least two characters after trimming.
                // The name must also contain at least one unicode 'letter'.

                if (s.Length < 1)
                    e("{0} is too short.  Please choose a name at least two characters in length.");
                else if (((s[0] >= 'a' && s[0] <= 'z') || (s[0] >= 'A' && s[0] <= 'Z')) && s.Length == 1)
                    e("{0} is too short.  Please choose a name at least two characters in length.");
                else if (s.Length > 16)
                    e("That name's pretty long!  Could you squeeze it down to no more than 16 characters?!");
                else if (s.All(c => !Char.IsLetter(c)))
                    e("{0} doesn't contain any letters.  Chuck one in for us, would you?");
                else if (s.Contains("'") || s.Contains('"') || s.EndsWith(".") || s.EndsWith(",") || s.EndsWith(";") || s.EndsWith(">"))
                    e("{0} contains a ' or a \" or ends with a '.', ',', '>', or ';'.  You can't do this - sorry.  It'd make the game look odd.");
                else if ((s.Count(c => c == '(') != s.Count(c => c == ')')) || (s.Count(c => c == '[') != s.Count(c => c == ']'))
                    || (s.Count(c => c == '{') != s.Count(c => c == '}')) || (s.Count(c => c == '<') != s.Count(c => c == '>')))
                    e("{0} contains mismatched brackets, a real pet hate of mine.  Please make sure there's the same number of open as close brackets for each bracket type.");
                else
                {
                    p.Tell("You entered your name as '{0}'.  Is this the handle you wish to use?  y/n >", s);
                    string yn = (await input.ReceiveAsync()).Trim().ToLower();

                    if (yn.StartsWith("y") || new List<string> { "yes", "y", "yep", "you bet", "of course", "absolutely",
                        "very yes", "very much so", "indubitably", "hell yes", "hell yeah", "yea", "ye" }.Contains(yn))
                        name = s;
                    else
                        p.Tell("Okay.  Let's have another go.");
                }
            }

            p.Tell("That's all I needed to know.  Let's begin!");

            // actually set the properties
            p.Name = name;
        }
    }
}
