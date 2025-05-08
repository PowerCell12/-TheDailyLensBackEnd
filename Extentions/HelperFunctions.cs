namespace server.Extentions;

public  static class HelperFunctions
{

    public static string StripHTML(string HTML){

        if (string.IsNullOrEmpty(HTML)) return HTML;

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(HTML);

        return doc.DocumentNode.InnerText;
    }
}