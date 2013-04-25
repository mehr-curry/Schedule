/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 03/28/2013
 * Time: 23:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ScheduleTest
{
	/// <summary>
	/// Description of DataMockUp.
	/// </summary>
	public class DataMockUp
	{
		public DataMockUp()
		{
            var mas = new ObservableCollection<Ma>();
            var aps = new ObservableCollection<Ap>();
            var pas = new ObservableCollection<Pa>();
            var les = new ObservableCollection<Le>();

		    for (int i = 0; i < 10; i++)
		    {
                var codeBase = i.ToString(CultureInfo.InvariantCulture);
		        mas.Add(CreateMa(codeBase));
                aps.Add(CreateAp(codeBase));
                pas.Add(CreatePa(codeBase));
                les.Add(CreateLe(codeBase, (i+1) * 5));
		    }

		    Mas = mas;
		    Aps = aps;
            Pas = pas;
		    Les = les;

            Termine = new ObservableCollection<Termin2>();
		}

        private Ma CreateMa(string i)
        {
            var result = new Ma() { Code = i, Name = "Ma" + i };
            return result;
        }

        private Ap CreateAp(string i)
        {
            var result = new Ap() { Code = i, Name = "Ap" + i };
            return result;
        }

        private Le CreateLe(string i, int dauer)
        {
            var result = new Le() { Code = i, Name = "Le" + i };
            return result;
        }

        private Pa CreatePa(string i)
        {
            var result = new Pa() { Code = i, Name = "Pa" + i };
            return result;
        }

        
	    public void AddTermin()
	    {
            EntriesTermin.Add(new Termin()
	                        {
	                            Datum = DateTime.Today,
	                            Von = new TimeSpan(6, 0, 0),
	                            Bis = new TimeSpan(7, 0, 0)
	                        });
	    }

        private ObservableCollection<IEntry> entries = null;
	    public ObservableCollection<IEntry> EntriesTermin
		{
			get
			{
                //var rnd = new Random();
                //var result = new ObservableCollection<IEntry>();

                //for (int i = 0; i < 100; i++)
                //{
                //    var von = new TimeSpan(rnd.Next(0, 23), rnd.Next(0, 59), 0);

                //    result.Add(new Termin()
                //    {
                //        Datum = new DateTime(2013, 4, rnd.Next(DateTime.Today.Day, DateTime.Today.Day + 7)),
                //        Von = von,
                //        Bis = new TimeSpan(von.Hours+1,von.Minutes,0)
                //    });
                //}

                //return result;

			    if (entries == null)
			    {
			        entries = new ObservableCollection<IEntry>()
			                      {
			                          new Termin()
			                              {
			                                  Datum = DateTime.Today,
			                                  Von = new TimeSpan(6, 0, 0),
			                                  Bis = new TimeSpan(7, 0, 0)
			                              },
			                          new Termin()
			                              {
			                                  Datum = DateTime.Today.AddDays(1),
			                                  Von = new TimeSpan(7, 0, 0),
			                                  Bis = new TimeSpan(8, 0, 0)
			                              },
			                          new Termin()
			                              {
			                                  Datum = DateTime.Today.AddDays(2),
			                                  Von = new TimeSpan(8, 0, 0),
			                                  Bis = new TimeSpan(10, 0, 0)
			                              },
			                          new Termin()
			                              {
			                                  Datum = DateTime.Today.AddDays(3),
			                                  Von = new TimeSpan(8, 0, 0),
			                                  Bis = new TimeSpan(10, 0, 0)
			                              },
			                          new Termin()
			                              {
			                                  Datum = DateTime.Today.AddDays(4),
			                                  Von = new TimeSpan(8, 0, 0),
			                                  Bis = new TimeSpan(10, 0, 0)
			                              }
			                      };
			    }

                return entries;
			} 
		}
		
		public IEnumerable HorizontalHeaderDatum
		{ 
			get
			{
				return new ObservableCollection<DateTime>() {DateTime.Today,
																DateTime.Today.AddDays(1),
																DateTime.Today.AddDays(2)};
			}
		}
		
		public IEnumerable VerticalHeaderZeit
		{ 
			get
			{
				return new ObservableCollection<TimeSpan>() {TimeSpan.Zero,
																new TimeSpan(1,0,0),
																new TimeSpan(2,0,0)};
			}
		}

	    public IEnumerable<Ma> Mas { get; set; }
	    public IEnumerable<Ap> Aps { get; set; }
	    public IEnumerable<Pa> Pas { get; set; }
        public IEnumerable<Le> Les { get; set; }
        public IEnumerable<Termin2> Termine { get; set; }

	}

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    public class Ressource : ViewModelBase
    {
        private string _code;
        public string Code
        {
            get { return _code; }
            set
            {
                if (value != _code)
                {
                    _code = value;
                    OnPropertyChanged("Code");
                }
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        
        
    }

    public class Le : Ressource
    {
        private TimeSpan _dauer;
        public TimeSpan Dauer
        {
            get { return _dauer; }
            set
            {
                if (value != _dauer)
                {
                    _dauer = value;
                    OnPropertyChanged("Dauer");
                }
            }
        }
        
    }
    public class Ap : Ressource{}
    public class Ma : Ressource{}
    public class Pa : Ressource{}


    public class Dp : CalendarItem
    {
        private IEnumerable<Ap> _ap;
        public IEnumerable<Ap> Aps
        {
            get { return _ap; }
            set
            {
                if (value != _ap)
                {
                    _ap = value;
                    OnPropertyChanged("Aps");
                }
            }
        }

        private IEnumerable<Ma> _mas;
        public IEnumerable<Ma> Mas
        {
            get { return _mas; }
            set
            {
                if (value != _mas)
                {
                    _mas = value;
                    OnPropertyChanged("Mas");
                }
            }
        }

        private IEnumerable<Le> _les;
        public IEnumerable<Le> Les
        {
            get { return _les; }
            set
            {
                if (value != _les)
                {
                    _les = value;
                    OnPropertyChanged("Les");
                }
            }
        }

        private IEnumerable<Pa> _pas;
        public IEnumerable<Pa> Pas
        {
            get { return _pas; }
            set
            {
                if (value != _pas)
                {
                    _pas = value;
                    OnPropertyChanged("Pas");
                }
            }
        }
        
    }

    public class CalendarItem : ViewModelBase
    {
        private DateTime _datum;
        private TimeSpan _startZeit;
        private TimeSpan _dauer;

        public DateTime Datum
        {
            get { return _datum; }
            set
            {
                if (value != _datum)
                {
                    _datum = value;
                    OnPropertyChanged("Datum");
                }
            }
        }

        public TimeSpan StartZeit
        {
            get { return _startZeit; }
            set
            {
                if (value != _startZeit)
                {
                    _startZeit = value;
                    OnPropertyChanged("StartZeit");
                }
            }
        }

        public TimeSpan Dauer
        {
            get { return _dauer; }
            set
            {
                if (value != _dauer)
                {
                    _dauer = value;
                    OnPropertyChanged("Dauer");
                }
            }
        }
    }

    public class Termin2 : CalendarItem
    {
        private Ap _ap;
        public Ap Ap
        {
            get { return _ap; }
            set
            {
                if (value != _ap)
                {
                    _ap = value;
                    OnPropertyChanged("Ap");
                }
            }
        }
        

        private IEnumerable<Ma> _mas;
        public IEnumerable<Ma> Mas
        {
            get { return _mas; }
            set
            {
                if (value != _mas)
                {
                    _mas = value;
                    OnPropertyChanged("Mas");
                }
            }
        }

        private Pa _pa;
        public Pa Pa
        {
            get { return _pa; }
            set
            {
                if (value != _pa)
                {
                    _pa = value;
                    OnPropertyChanged("Pa");
                }
            }
        }

        private Le _le;
        public Le Le
        {
            get { return _le; }
            set
            {
                if (value != _le)
                {
                    _le = value;
                    OnPropertyChanged("Le");
                }
            }
        }
    }

    public class UiViewModel : ViewModelBase
    {
        private IEnumerable<Ma> _selectedMas;
        public IEnumerable<Ma> SelectedMas
        {
            get { return _selectedMas; }
            set
            {
                if (value != _selectedMas)
                {
                    _selectedMas = value;
                    OnPropertyChanged("SelectedMas");
                    OnPropertyChanged("SelectedRessourcen");
                }
            }
        }

        private IEnumerable<Ap> _selectedAps;
        public IEnumerable<Ap> SelectedAps
        {
            get { return _selectedAps; }
            set
            {
                if (value != _selectedAps)
                {
                    _selectedAps = value;
                    OnPropertyChanged("SelectedAps");
                    OnPropertyChanged("SelectedRessourcen");
                }
            }
        }

        private IEnumerable<Pa> _selectedPas;
        public IEnumerable<Pa> SelectedPas
        {
            get { return _selectedPas; }
            set
            {
                if (value != _selectedPas)
                {
                    _selectedPas = value;
                    OnPropertyChanged("SelectedPas");
                    OnPropertyChanged("SelectedRessourcen");
                }
            }
        }

        private IEnumerable<Le> _selectedLes;
        public IEnumerable<Le> SelectedLes
        {
            get { return _selectedLes; }
            set
            {
                if (value != _selectedLes)
                {
                    _selectedLes = value;
                    OnPropertyChanged("SelectedLes");
                    OnPropertyChanged("SelectedRessourcen");
                }
            }
        }

        private readonly RessourceEqualityComparer _comparer = new RessourceEqualityComparer();

        public IEnumerable<Ressource> SelectedRessourcen
        {
            get
            {
                IEnumerable<Ressource> result = new ObservableCollection<Ressource>();
                if (SelectedMas != null && SelectedMas.Any()) result = result.Union(SelectedMas, _comparer);
                if (SelectedPas != null && SelectedPas.Any()) result = result.Union(SelectedPas, _comparer);
                if (SelectedAps != null && SelectedAps.Any()) result = result.Union(SelectedAps, _comparer);
                if (SelectedLes != null && SelectedLes.Any()) result = result.Union(SelectedLes, _comparer);
                return result.OrderBy(r => r.GetType().Name + r.Code);
            }
        }
        
    }

    internal class RessourceEqualityComparer : IEqualityComparer<Ressource>
    {
        public bool Equals(Ressource x, Ressource y)
        {
            return Object.Equals(x, y);
        }

        public int GetHashCode(Ressource obj)
        {
            return obj.GetHashCode();
        }
    }
}
