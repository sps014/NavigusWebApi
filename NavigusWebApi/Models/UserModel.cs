using Google.Cloud.Firestore;

namespace NavigusWebApi.Models
{
    public class NewUserModel
    {
        public string? Email {  get; set; }
        public string? Password {  get; set; }
        public Roles Role { get; set; }
    }
    public class UserModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    [FirestoreData]
    public class UserInfo
    {
        [FirestoreProperty]
        public Roles Role { get; set; }
        [FirestoreProperty]
        public string? Password { get; set; }
    }

    public enum Roles
    {
        Student, Teacher
    }
}
