﻿using Google.Cloud.Firestore;

namespace NavigusWebApi.Models
{
    [FirestoreData]
    public class QuestionModel
    {
        [FirestoreProperty]
        public string Question { get; set; }

        [FirestoreProperty]
        public string[] Options { get; set; }

        [FirestoreProperty]
        public uint Points { get; set; }

        [FirestoreProperty]
        public int[] CorrectOptionIndex { get; set; }
    }
}