using FootprintViewer.Localization;
using System.Runtime.Serialization;

namespace FootprintViewer.AppStates
{
    [DataContract]
    public class LocalizationState
    {
        [DataMember]
        public LanguageModel? Language { get; set; }
    }
}
