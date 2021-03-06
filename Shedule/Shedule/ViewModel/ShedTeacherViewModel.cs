﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.IO;

using Shedule.Commands;
using Shedule.Import;
using Shedule.Data;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using Shedule.Common;

namespace Shedule.ViewModel
{
    class ShedTeacherViewModel : ViewModelBase
    {
        #region тип недели, дата
        private bool upweek;
        public bool UpWeek
        {
            get { return upweek; }
            set
            {
                upweek = value;
                OnPropertyChanged("UpWeek");
                ChangeWeekTypeHandler();
            }
        }

        private bool downweek;
        public bool DownWeek
        {
            get { return downweek; }
            set
            {
                downweek = value;
                OnPropertyChanged("DownWeek");
            }
        }

        DateTime selecteddate;
        public DateTime SelectedDate
        {
            get { return selecteddate; }
            set
            {
                selecteddate = value;
                OnPropertyChanged("SelectedDate");
                DateSelectedHandler();
            }
        }
        #endregion

        ObservableCollection<Employe> teachers;
        public ObservableCollection<Employe> Teachers
        {
            get { return teachers; }
            set
            {
                teachers = value;
                OnPropertyChanged("Teachers");
            }
        }

        ObservableCollection<DisplayCurriculumLesson> lessons;
        public ObservableCollection<DisplayCurriculumLesson> Lessons
        {
            get { return lessons; }
            set
            {
                lessons = value;
                OnPropertyChanged("Lessons");
            }
        }

        Employe selectedTeacher;
        public Employe SelectedTeacher
        {
            get { return selectedTeacher; }
            set
            {
                selectedTeacher = value;
                OnPropertyChanged("SelectedTeacher");
                TeacherSelectedHandler();
            }
        }

        /// <summary>
        /// 
        /// </summary>

        public ShedTeacherViewModel()
        {
            lessons = new ObservableCollection<DisplayCurriculumLesson>();
            UpWeek = true;
            SelectedDate = DateTime.Today;
            FillTeachersList();
        }

        private void FillTeachersList()
        {
            using (UniversitySheduleContainer cnt = new UniversitySheduleContainer("name=UniversitySheduleContainer"))
            {
                var tea = (from t in cnt.Employees select t).OrderBy(t => t.Name);
                Teachers = new ObservableCollection<Employe>(tea);
            }
        }

        private void FillLessonsList()
        {
            lessons.Clear();
            for (int i = 0; i < 49; ++i)
            {
                lessons.Add(new DisplayCurriculumLesson());
            }

            if (selectedTeacher == null) return;

            using (UniversitySheduleContainer cnt = new UniversitySheduleContainer("name=UniversitySheduleContainer"))
            {
                var les = (from l in cnt.Lessons.Include("RegulatoryAction.AcademicLoad").Include("RegulatoryAction.Curriculum") where l.Period == upweek select l);
                foreach (var l in les)
                {
                    foreach (var a in l.RegulatoryAction.AcademicLoad)
                    {
                        if (a.EmployeId == selectedTeacher.Id)
                        {
                            int i = HelperClasses.numberDayToIndex(l.Day,l.RingId);
                            lessons[i]._Subject = l.RegulatoryAction.Curriculum.First().Subject.Name;
                            lessons[i]._Type = l.RegulatoryAction.LessonsType.Name;
                            lessons[i]._Auditorium = "ауд. " + l.Auditorium.Number;
                            foreach (var c in l.RegulatoryAction.Curriculum)
                            {
                                lessons[i]._Group += c.Group.GroupAbbreviation + " ";
                            }
                            break;
                        }
                    }
                }
            }

            Lessons = new ObservableCollection<DisplayCurriculumLesson>(lessons);
        }

        #region неделя -
        private DelegateCommand prevweek;

        public ICommand PrevWeekCommand
        {
            get
            {
                if (prevweek == null)
                {
                    prevweek = new DelegateCommand(PrevWeek);
                }
                return prevweek;
            }
        }
        public void PrevWeek()
        {
            SelectedDate = SelectedDate.AddDays(-7);
        }
        #endregion

        #region неделя +
        private DelegateCommand nextweek;

        public ICommand NextWeekCommand
        {
            get
            {
                if (nextweek == null)
                {
                    nextweek = new DelegateCommand(NextWeek);
                }
                return nextweek;
            }
        }
        public void NextWeek()
        {
            SelectedDate = SelectedDate.AddDays(7);
        }
        #endregion

        #region обработчики на изменении свойств
        private void TeacherSelectedHandler()
        {
            FillLessonsList();
        }

        private void ChangeWeekTypeHandler()
        {
            FillLessonsList();
        }

        private void DateSelectedHandler()
        {
        }
        #endregion
    }
}
