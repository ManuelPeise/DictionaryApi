namespace Logic.Words
{
    internal class Constants
    {
        internal const string KaikkiDumpFileUrl = "https://kaikki.org/dictionary/raw-wiktextract-data.jsonl.gz";
      
        internal const string KaikkiFileNameTemplate = "{Type}_Kaikki_{TimeStamp}.json";
        internal const string KaikkiJsonBackupFileName = "Kaikki_Backup.json";
        internal const string DumpFilePrefix = "Dump";
        internal const string KaikkiBackupDateFormat = "ddMMyyyy";
        internal static readonly List<string> ValidLanguages = new List<string> { "da", "de", "en" };
        internal static readonly List<string> ValidPartsOfSpeech = new List<string>
        {
            "adj",
            "adjective",
            "adv",
            "adverb",
            "article",
            "noun",
            "preposition",
            "proper noun",
            "pronoun",
            "verb"
        };

    }
}
