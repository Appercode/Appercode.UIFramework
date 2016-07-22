using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Appercode.Samples.Model
{
    public class TestModel : INotifyPropertyChanged
    {
        private readonly IList<string> carriers;

        private string carrier;

        public TestModel()
        {
            var random = new Random();
            carriers = GetCarriers().OrderBy(c => c, StringComparer.OrdinalIgnoreCase).ToArray();
            carrier = carriers[random.Next(carriers.Count)];
            FirstName = "John";
            LastName = "Smith";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Carrier
        {
            get { return carrier; }
            set
            {
                if (carrier != value)
                {
                    carrier = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Carrier)));
                }
            }
        }

        public IEnumerable Carriers => carriers;

        private IEnumerable<string> GetCarriers()
        {
            yield return "Aetna";
            yield return "Cigna";
            yield return "Humana";
            yield return "United Healthcare";
            yield return "Anthem Blue Cross";
            yield return "Blue Shield";
            yield return "Absolute Total Care";
            yield return "Affinity Health Plan";
            yield return "Aftra Health Fund";
            yield return "All Savers Insurance";
            yield return "Altius Health Plans";
            yield return "American Medical Security";
            yield return "American Republic Insurance Company";
            yield return "Amerigroup";
            yield return "AmeriHealth Caritas";
            yield return "Ameritas";
            yield return "Arnett Health Plan";
            yield return "Assurant Health";
            yield return "Best Life and Health";
            yield return "Better Health Plan (Unison)";
            yield return "Regence BlueCross BlueShield";
            yield return "Boston Medical Center Health Plan";
            yield return "Bridgeway Health Solutions";
            yield return "Buckeye Community Health Plan";
            yield return "Carelink (Coventry)";
            yield return "CarePlus Health Plan";
            yield return "CareSource";
            yield return "Cariten Healthcare";
            yield return "CeltiCare Health Plan";
            yield return "Cenpatico Behavioral Health";
            yield return "CHAMPVA";
            yield return "Chesapeake Life Insurance";
            yield return "Community Hospital Corporation (CHC)";
            yield return "Complementary Healthcare Plan";
            yield return "ConnectiCare";
            yield return "Cooperative Benefit Administrators";
            yield return "Coordinated Care";
            yield return "CoreSource";
            yield return "CoventryCares";
            yield return "Coventry Health Care";
            yield return "Cypress Benefits";
            yield return "Definity Health (United)";
            yield return "Directors Guild of America Health Fund";
            yield return "Emblem Health";
            yield return "The Empire Plan (United)";
            yield return "Fallon Community Health Plan";
            yield return "Federated Mutual Insurance Company";
            yield return "Fidelis Care";
            yield return "First Health (Coventry)";
            yield return "Florida Health Care Plan";
            yield return "Government Employees Health Association";
            yield return "Golden Rule";
            yield return "Great West Healthcare";
            yield return "Harmony Health Plan";
            yield return "Harvard Pilgrim Health Care";
            yield return "Harvard Pilgrim Passport Connect (United)";
            yield return "Health America (Coventry)";
            yield return "Health Choice";
            yield return "Health Net";
            yield return "HealthPartners Minnesota";
            yield return "Health Partners Plans, Inc";
            yield return "Healthcare USA (Coventry)";
            yield return "HealthEase (WellCare)";
            yield return "Healthfirst";
            yield return "HealthSmart Benefit Solutions";
            yield return "HHIC Freedom Blue";
            yield return "Highmark Blue Cross Blue Shield";
            yield return "Horizon NJ Health";
            yield return "Hudson Health Plan";
            yield return "Illinicare health Plan";
            yield return "John Hopkins HealthCare LLC";
            yield return "Kaiser Permanente";
            yield return "Kentucky Spirit";
            yield return "Keystone First";
            yield return "Louisiana Healthcare Connections";
            yield return "LifeWise Health Plan";
            yield return "Magellan Health";
            yield return "Magnolia Health Plan";
            yield return "MAHP MAMSI (United)";
            yield return "Mail Handlers Benefit Plan (MHBP)";
            yield return "Managed Health Services";
            yield return "Medica Health Plans";
            yield return "Medical Mutual";
            yield return "Mega Life and Health Insurance";
            yield return "Mid-West National Life Insurance Company";
            yield return "Molina Healthcare";
            yield return "MVP Health Care";
            yield return "National Association of Letter Carriers";
            yield return "Nationwide Health Plan";
            yield return "Neighborhood Health Plan";
            yield return "Network Health";
            yield return "Nippon Life";
            yield return "Ohana Health Plan";
            yield return "OptumHealth Behavioral Solutions";
            yield return "Oxford Health Plan";
            yield return "Paramount";
            yield return "Peach State Health Plan";
            yield return "PEHP Utah";
            yield return "Physicians Mutual Insurance Company";
            yield return "Premera Blue Cross";
            yield return "Principal Financial Group";
            yield return "QualChoice";
            yield return "SelectHealth";
            yield return "Sierra Health Plan";
            yield return "StayWell";
            yield return "Sunflower State Health Plan";
            yield return "Sunshine State Health Plan";
            yield return "Superior Health Plan";
            yield return "Transamerica Life";
            yield return "Trustmark";
            yield return "Tufts Health Plan";
            yield return "UHC West (PacifiCare)";
            yield return "UMR";
            yield return "UniCare";
            yield return "University of Missouri Health (Coventry)";
            yield return "UPMC Health Plan";
            yield return "USAA Life Insurance";
            yield return "VA Fee Basis Program";
            yield return "Wellcare";
            yield return "Wellmark Blue Cross";
            yield return "Western Health Advantage";
            yield return "Writers Guild Health Fund";
            yield return "Medicare";
        }
    }
}
