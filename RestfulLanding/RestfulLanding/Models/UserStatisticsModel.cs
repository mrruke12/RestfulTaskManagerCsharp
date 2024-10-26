namespace RestfulLanding.Models {
    public class UserStatisticsModel {
        public int inwork { get; set; }
        public int none { get; set; }
        public int done { get; set; }
        public int total { get; set; }
        public string noneLabel { get; set; } = LocalizationManager.current["StatusNone"];
        public string inworkLabel { get; set; } = LocalizationManager.current["StatusInWork"];
        public string doneLabel { get; set; } = LocalizationManager.current["StatusDone"];
        public string totalLabel { get; set; } = LocalizationManager.current["TotalLabel"];
    }
}
