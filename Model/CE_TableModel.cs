using System;

namespace CE_Tracker.Model
{
    public class CE_TableModel : ModelBase
    {
        //Primary key
        private int _modId;
        public int ModId
        {
            get { return _modId;  }
            set { _modId = value; OnPropertyChanged(); }
        }

        // UserId, foreign key to Users table
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        private DateTime _ceMod;
        public DateTime CEmod
        {
            get { return _ceMod; }
            set { _ceMod = value; OnPropertyChanged(); }
        }
       

        private string _subject;
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; OnPropertyChanged(); }
        }

        private float _hours;
        public float Hours
        {
            get { return _hours; }
            set { _hours = value; OnPropertyChanged(); }
        }

        private string _comments;

        public string Comments
        {
            get { return _comments; }
            set { _comments = value; OnPropertyChanged(); }
        }


        public CE_TableModel(int ModId, int Id, DateTime CEmod, string Subject, float Hours, string Comments)
        {
            this.ModId = ModId;
            this.Id = Id;
            this.CEmod = CEmod;
            this.Subject = Subject;
            this.Hours = Hours;
            this.Comments = Comments;   

        }

        public CE_TableModel()
        {

        }

 
    }

    

}
