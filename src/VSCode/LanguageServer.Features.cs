using VSCode.Completion;
using VSCode.Editor;
using VSCode.Notification;

namespace VSCode
{
    public partial class LanguageServer
    {
        public EditorFeature Editor => GetFeature<EditorFeature>();
        public NotificationFeature Notifications => GetFeature<NotificationFeature>();
        public CompletionFeature Completion => GetFeature<CompletionFeature>();
    }
}
