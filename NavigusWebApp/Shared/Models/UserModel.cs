using Google.Cloud.Firestore;

namespace NavigusWebApp.Shared.Models
{
    public class NewUserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public Roles Role { get; set; }
    }
    public class UserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }


    [FirestoreData]
    public class UserInfo
    {
        [FirestoreProperty]
        public Roles Role { get; set; }
        [FirestoreProperty]
        public string Password { get; set; }
        [FirestoreProperty]
        public string UserName { get; set; }
        [FirestoreProperty]
        public string Email { get; set; }
    }

    public enum Roles
    {
        Student, Teacher
    }
}
