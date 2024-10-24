using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;


namespace RestfulLanding.Localization
{
    public abstract class Localization
    {
        protected Dictionary<string, string> _localization = new Dictionary<string, string>();
        public bool initialized = false;
        public string language { get; init; }
        public string transcription { get; init; }

        protected async Task Deserialize (string path) {
            string json = await File.ReadAllTextAsync(path);
            if (JsonSerializer.Deserialize<Dictionary<string, string>>(json) is Dictionary<string, string> dict) _localization = dict;
            else throw new InvalidDataException($"Error occured while deserialize {language} json source");
            initialized = true;
        }

        public string this[string key] {
            get {
                if (!initialized) throw new NullReferenceException($"Null reference occured within indexator at {language} localization");
                return _localization[key]; 
            }
            set { 
                if (!initialized) throw new NullReferenceException($"Null reference occured within indexator at {language} localization");
                _localization[key] = value; 
            }
        }
    }

    public class RuLocalization : Localization {
        public static readonly Lazy<RuLocalization> _instance = new Lazy<RuLocalization>(() => new RuLocalization());
        public RuLocalization() {
            language = "RU";
            transcription = "Русский";
        }

        public async Task Initialize() {
            if (!RuLocalization.Instance().initialized) {
                await Deserialize("Localization/RU.json");
                LocalizationManager.localizations.Add(RuLocalization.Instance());
            }
        }
        public static RuLocalization Instance() => _instance.Value;
    }

    public class EnLocalization : Localization {
        public static readonly Lazy<EnLocalization> _instance = new Lazy<EnLocalization>(() => new EnLocalization());

        public EnLocalization() {
            language = "EN";
            transcription = "English";
        }

        public async Task Initialize() {
            if (!EnLocalization.Instance().initialized) {
                await Deserialize("Localization/EN.json");
                LocalizationManager.localizations.Add(EnLocalization.Instance());
            }
        }
        public static EnLocalization Instance() => _instance.Value;
    }

    public static class LocalizationManager {
        public static Localization current;
        public static List<Localization> localizations = new List<Localization>();
        public static void Set(Localization value) { current = value; }
        public static void Set(string value) {
            foreach (Localization localization in localizations) {
                if (localization.language == value) {
                    current = localization;
                    break;
                }
            }
        }
    }
}
