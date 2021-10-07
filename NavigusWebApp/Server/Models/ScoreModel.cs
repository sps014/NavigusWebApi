using Google.Cloud.Firestore;

namespace NavigusWebApi.Models
{
    [FirestoreData]
    public class ScoreModel
    {
        [FirestoreProperty]
        public string Uid {  get; set; }
        [FirestoreProperty]
        public int Points {  get; set; }
        [FirestoreProperty]
        public int Xp { get; set; }
    }
}
