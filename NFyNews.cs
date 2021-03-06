using System.Collections.Specialized;
using Godot;
using Godot.Collections;
using System;
using System.Text;

public class NFyNews : Panel
{
    public bool TryForNFyMono = false;
    public System.Collections.Generic.Dictionary<string, string> ConverNewsText(string t) {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, string>>(t);
    }
    // The news!
    public override void _Ready()
    {
        GetNode("HTTPRequest").Connect("request_completed", this, "OnRequestCompleted");

        HTTPRequest httpRequest = GetNode<HTTPRequest>("HTTPRequest");
        Console.WriteLine("Requesting: " + "https://thekaigonzalez.github.com/NFyMono/news/" + DateTime.Now.Month + "-" +  DateTime.Now.Day + "-" + DateTime.Now.Year + ".json");
        httpRequest.Request("https://thekaigonzalez.github.io/NFyMono/news/" + DateTime.Now.Month + "-" +  DateTime.Now.Day + "-" + DateTime.Now.Year + ".json");
        
    }
    
    public void OnRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        Console.WriteLine(Encoding.UTF8.GetString(body));
        if (response_code == 200) {
            string s = Encoding.UTF8.GetString(body);
            var msn = ConverNewsText(s);
            Console.WriteLine("Got news, applying");
            GetNode<TextEdit>("news").Text = msn["body"].Trim();
            GetNode<Label>("author").Text = msn["author"].Trim();
            
        } else {

        }
    }
}
