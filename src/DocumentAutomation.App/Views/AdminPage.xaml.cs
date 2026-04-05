using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Domain.Security;
using System.Windows;
using System.Windows.Controls;

namespace DocumentAutomation.App.Views;

public partial class AdminPage : UserControl
{
    private readonly ISecurityAdministrationService _securityAdministrationService;
    private IReadOnlyList<User> _users = Array.Empty<User>();
    private IReadOnlyList<Role> _roles = Array.Empty<Role>();

    public AdminPage(ISecurityAdministrationService securityAdministrationService)
    {
        _securityAdministrationService = securityAdministrationService;
        InitializeComponent();
        Loaded += HandleLoadedAsync;
    }

    private async void HandleLoadedAsync(object sender, RoutedEventArgs e)
    {
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        _users = await _securityAdministrationService.GetUsersAsync();
        _roles = await _securityAdministrationService.GetRolesAsync();

        UsersGrid.ItemsSource = _users;
        RolesList.ItemsSource = _roles;
        UpdateSelectedUserSummary();
    }

    private void OnUserSelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateSelectedUserSummary();

    private async void OnAssignRoleClicked(object sender, RoutedEventArgs e)
    {
        if (UsersGrid.SelectedItem is not User user || RolesList.SelectedItem is not Role role)
        {
            AdminStatusText.Text = "Select both a user and a role before assigning.";
            return;
        }

        await _securityAdministrationService.AssignRoleAsync(user.Id, role.Id);
        AdminStatusText.Text = $"Assigned role '{role.Name}' to '{user.UserName}'.";
        await RefreshAsync();
    }

    private void UpdateSelectedUserSummary()
    {
        if (UsersGrid.SelectedItem is not User user)
        {
            AssignedRolesText.Text = "Select a user to inspect role assignments.";
            return;
        }

        var roles = user.UserRoles.Select(item => item.Role?.Name ?? item.RoleId.ToString()).ToList();
        AssignedRolesText.Text = roles.Count == 0
            ? "No roles assigned."
            : string.Join(Environment.NewLine, roles);
    }
}
