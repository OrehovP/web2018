using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vacancies.Models
{
    public class VacanciesViewModel
	{
		public VacanciesViewModel(int bigSalary, int lowSalary)
		{
			BigSalary = bigSalary;
			LowSalary = lowSalary;
			ProfessionsBigSalary = new List<string>();
			SkillsBigSalary = new List<string>();
			ProfessionsLowSalary = new List<string>();
			SkillsLowSalary = new List<string>();
		}

		public IList<string> ProfessionsBigSalary { get; private set; }
		public IList<string> SkillsBigSalary { get; private set; }
		public IList<string> ProfessionsLowSalary { get; private set; }
		public IList<string> SkillsLowSalary { get; private set; }
		public int BigSalary { get; private set; }
		public int LowSalary { get; private set; }
	}
}
