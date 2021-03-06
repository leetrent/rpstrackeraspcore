using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RPS.Core.Models.Dto;
using RPS.Core.Models;
using RPS.Data;

namespace RPS.Web.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly IPtDashboardRepository rpsDashRepo;
        private readonly IPtUserRepository rpsUserRepo;

        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        public int IssueCountOpen { get; set; }
        public int IssueCountClosed { get; set; }

        public int IssueCountActive { get { return IssueCountOpen + IssueCountClosed; } }
        public decimal IssueCloseRate { 
            get 
            { 
                if (IssueCountActive == 0)
                {
                    return 0m;
                }
                return Math.Round((decimal)IssueCountClosed / (decimal)IssueCountActive * 100m, 2); 
            } 
        }

        public int? SelectedAssigneeId { get; set; }
        public List<PtUser> Assignees { get; set; }

        //////////////////////////////////////////////////////////////////////////////////////////
        // For Chart
        //////////////////////////////////////////////////////////////////////////////////////////
        public object[] Categories { get; set; } // Object array is required if using tag helpers
        public List<int> ItemsOpenByMonth { get; set; }
        public List<int> ItemsClosedByMonth { get; set; }
        //////////////////////////////////////////////////////////////////////////////////////////

        public DashboardModel(IPtDashboardRepository rpsDashData, IPtUserRepository rpsUserData)
        {
            rpsDashRepo = rpsDashData;
            rpsUserRepo = rpsUserData;
        }

        public void OnGet(int? userId, int? months)
        {
            ViewData.Add("userId", userId);
            ViewData.Add("months", months);

            DateTime start = months.HasValue ? DateTime.Now.AddMonths(months.Value * -1) : DateTime.Now.AddYears(-5);
            DateTime end = DateTime.Now;

            PtDashboardFilter filter = new PtDashboardFilter
            {
                DateStart = start,
                DateEnd = end,
                UserId = userId.HasValue ? userId.Value : 0
            };

            var statusCounts = rpsDashRepo.GetStatusCounts(filter);
            IssueCountOpen = statusCounts.OpenItemsCount;
            IssueCountClosed = statusCounts.ClosedItemsCount;

            if (months.HasValue)
            {
                DateStart = filter.DateStart;
                DateEnd = filter.DateEnd;
            }

            Assignees = rpsUserRepo.GetAll().ToList();
            if (userId.HasValue) 
            {
                SelectedAssigneeId = userId.Value;
            }

            /////////////////////////////////////////////////////////////////////////////////
            // Chart related
            /////////////////////////////////////////////////////////////////////////////////
            var filteredIssues = rpsDashRepo.GetFilteredIssues(filter);
            this.ItemsOpenByMonth = new List<int>();
            this.ItemsClosedByMonth = new List<int>();

            filteredIssues.MonthItems.ForEach(mi => {
                this.ItemsOpenByMonth.Add(mi.Open.Count);
                this.ItemsClosedByMonth.Add(mi.Closed.Count);
            });


            this.Categories = filteredIssues.Categories.Select(cat => (object)cat).ToArray();
        }
    }
}