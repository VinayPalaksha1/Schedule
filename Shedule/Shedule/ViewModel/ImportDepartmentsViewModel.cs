﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.IO;

using System.Collections.ObjectModel;
using Shedule.Import;
using Shedule.Data;
using Shedule.Commands;

namespace Shedule.ViewModel
{
    class ImportDepartmentsViewModel : ViewModelBase
    {
        private bool selectall; // 
        public bool SelectAll
        {
            get { return selectall; }
            set
            {
                selectall = value;
                OnPropertyChanged("SelectAll");
            }
        }

        ObservableCollection<ReadedDepartment> readeddepartment;
        public ObservableCollection<ReadedDepartment> ReadedDepartments
        {
            get { return readeddepartment; }
            set
            {
                readeddepartment = value;
                OnPropertyChanged("ReadedDepartments");
            }
        }

        private string inputfilename; // имя входного файла 
        public string InputFileName
        {
            get { return inputfilename; }
            set
            {
                inputfilename = value;
                OnPropertyChanged("InputFileName");
            }
        }

        #region Конструктор
        public ImportDepartmentsViewModel()
        {
            SelectAll = true;
        }
        #endregion

        #region Import
        private DelegateCommand import;

        public ICommand ImportCommand
        {
            get
            {
                if (import == null)
                {
                    import = new DelegateCommand(Import);
                }
                return import;
            }
        }

        public void Import()
        {
            using (UniversitySheduleContainer cnt = new UniversitySheduleContainer("name=UniversitySheduleContainer"))
            {
                foreach (var g in readeddepartment)
                {
                    if (g.Add)
                    {
                        Department newDepartment = new Department()
                        {
                            Abbreviation = g.Name,
                            Name = g.Name,
                            //Id = g.Id,
                            FacultyId = 1,
                        };
                        cnt.Departments.AddObject(newDepartment);
                    }
                }
                cnt.SaveChanges();
                Department virtualDepartment = new Department()
                {
                    Abbreviation = "VIRT",
                    Name = "Virtual",
                    FacultyId = 1,
                };
                cnt.Departments.AddObject(virtualDepartment);
                Auditorium virtualAudit = new Auditorium()
                {
                    Building = 1,
                    Number = "VIRTUAL",
                    Seats = 9999999,
                    Department = virtualDepartment,
                    OpeningDate = "",
                    ClosingDate = "",
                };
                cnt.Auditoriums.AddObject(virtualAudit);
                cnt.SaveChanges();
            }
        }
        #endregion

        #region Applay
        private DelegateCommand applay;

        public ICommand ApplayCommand
        {
            get
            {
                if (applay == null)
                {
                    applay = new DelegateCommand(Applay);
                }
                return applay;
            }
        }

        public void Applay()
        {
            DepartmentReader reader = new DepartmentReader();
            List<ReadedDepartment> result = reader.ReadFile(InputFileName);
            if (selectall)
            {
                foreach (var r in result)
                {
                    r.Add = true;
                }
            }
            ReadedDepartments = new ObservableCollection<ReadedDepartment>(result);
        }
        #endregion

        #region Выбор файла
        private DelegateCommand fileselect;

        public ICommand FileSelectCommand
        {
            get
            {
                if (fileselect == null)
                {
                    fileselect = new DelegateCommand(FileSelect);
                }
                return fileselect;
            }
        }
        public void FileSelect()
        {
            InputFileName = Reader.OpenFile();
            Applay();
        }
        #endregion
    }
}
