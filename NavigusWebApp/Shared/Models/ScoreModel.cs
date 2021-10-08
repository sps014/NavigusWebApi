using Google.Cloud.Firestore;

namespace NavigusWebApp.Shared.Models
{
    [FirestoreData]
    public class ScoreModel
    {
        [FirestoreProperty]
        public string Uid { get; set; }
        [FirestoreProperty]
        public int Points { get; set; }
        [FirestoreProperty]
        public int Xp { get; set; }
    }
}
