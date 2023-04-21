using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent.ViewModels.AddPlannedSchedule.Items;

public class PlannedScheduleItemViewModel
{
	public PlannedScheduleItemViewModel(string name, DateTime dateTime)
	{
		Name = name;

		DateTime = dateTime;
	}

	public string Name { get; set; }

	public DateTime DateTime { get; set; }
}
