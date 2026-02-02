using WhateverDotNet.TestApp.Components.Commands;
using WhateverDotNet.TestApp.Excel.Stores;
using WhateverDotNet.TestApp.Excel.ViewModels;

namespace WhateverDotNet.TestApp.Excel.Commands
{
    public class LoadReportSpecificationsCommand : BaseAsyncCommand
    {
        private readonly ReportSpecificationsViewModel _reportSpecificationsViewModel;
        private readonly ReportSpecificationsStore _reportsStore;

        public LoadReportSpecificationsCommand(ReportSpecificationsViewModel reportSpecificationsViewModel, ReportSpecificationsStore reportsStore)
        {
            _reportSpecificationsViewModel = reportSpecificationsViewModel
                ?? throw new ArgumentNullException(nameof(reportSpecificationsViewModel));
            _reportsStore = reportsStore
                ?? throw new ArgumentNullException(nameof(reportsStore));
        }

        public override async Task ExecuteAsync(object parameter)
        {
            _reportSpecificationsViewModel.ErrorMessage = null;
            _reportSpecificationsViewModel.IsLoading = true;

            try
            {
                await _reportsStore.Load();
            }
            catch (Exception)
            {
                // TODO:
                _reportSpecificationsViewModel.ErrorMessage = "Failed to load report specifications.";
            }
            finally
            {
                _reportSpecificationsViewModel.IsLoading = false;
            }
        }
    }
}