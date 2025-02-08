using DynamicData;
using NetworkManagerApp.Interfaces;
using NetworkManagerApp.Models;
using NetworkManagerApp.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reactive;

namespace NetworkManagerApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private NetworkInterfaceModel? _selectedInterface;
        private INetworkConfigurationService _networkConfigurationService;

        private string _newIPv4Address = string.Empty;
        private Ipv4Model? _selectedIPv4Address;
        private string _gateway = string.Empty;
        private string _dns = string.Empty;

        private string _validationMessage = string.Empty;
        private string _messageColor = string.Empty;
        private bool _isBusy;

        #endregion

        #region Proprieties
        public ObservableCollection<NetworkInterfaceModel> NetworkInterfaces { get; set; }

        public NetworkInterfaceModel? SelectedInterface
        {
            get => _selectedInterface;
            set => this.RaiseAndSetIfChanged(ref _selectedInterface, value);
        }

        public string NewIPAddress
        {
            get => _newIPv4Address;
            set
            {
                this.RaiseAndSetIfChanged(ref _newIPv4Address, value);
            }
        }

        public Ipv4Model? SelectedIPv4Address
        {
            get => _selectedIPv4Address;
            set => this.RaiseAndSetIfChanged(ref _selectedIPv4Address, value);
        }

        public string Gateway
        {
            get => _gateway;
            set => this.RaiseAndSetIfChanged(ref _gateway, value);
        }

        public string DNS
        {
            get => _dns;
            set => this.RaiseAndSetIfChanged(ref _dns, value);
        }

        public string? ValidationMessage
        {
            get => _validationMessage;
            set => this.RaiseAndSetIfChanged(ref _validationMessage, value);
        }

        public string MessageColor
        {
            get => _messageColor;
            set => this.RaiseAndSetIfChanged(ref _messageColor, value);
        }

        #region ReactiveCommands

        public ReactiveCommand<Unit, Unit> SetIPv4Command { get; }
        public ReactiveCommand<Unit, Unit> RemoveIPv4Command { get; }
        public ReactiveCommand<Unit, Unit> AddIPv4Command { get; }
        public ReactiveCommand<Unit, Unit> SetAutomaticIPv4Command { get; }
        public ReactiveCommand<Unit, Unit> SetGatewayCommand { get; }
        public ReactiveCommand<Unit, Unit> SetAutomaticGatewayCommand { get; }
        public ReactiveCommand<Unit, Unit> SetDNSCommand { get; }
        public ReactiveCommand<Unit, Unit> SetAutomaticDNSCommand { get; }

        #endregion
        #endregion

        #region Constructors
        public MainWindowViewModel(INetworkConfigurationService networkConfigurationService)
        {
            _networkConfigurationService = networkConfigurationService;

            LoadNetworkInterfaces();

            SetIPv4Command = ReactiveCommand.Create(SetIPv4Address);
            RemoveIPv4Command = ReactiveCommand.Create(RemoveIPv4Address);
            AddIPv4Command = ReactiveCommand.Create(AddIPv4Address);
            SetAutomaticIPv4Command = ReactiveCommand.Create(SetAutomaticIPv4Address);
            SetGatewayCommand = ReactiveCommand.Create(SetGateway);
            SetAutomaticGatewayCommand = ReactiveCommand.Create(SetAutomaticGateway);
            SetDNSCommand = ReactiveCommand.Create(SetDNS);
            SetAutomaticDNSCommand = ReactiveCommand.Create(SetAutomaticDNS);
        }
        #endregion

        #region Methods
        private void LoadNetworkInterfaces()
        {
            if (NetworkInterfaces is null)
            {
                NetworkInterfaces = new();
            }
            else
            {
                NetworkInterfaces.Clear();
            }

            var interfaces = _networkConfigurationService.GetNetworkInterfaces();

            foreach (var netInterface in interfaces)
            {
                NetworkInterfaces.Add(netInterface);
            }
        }

        private bool ValidateIPv4Address(string ipAddress)
        {
            string ipv4Pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

            if (System.Text.RegularExpressions.Regex.IsMatch(ipAddress, ipv4Pattern))
            {
                return true;
            }
            else
            {
                ValidationMessage = "O endereço IPv4 não é válido. Por favor, insira um endereço válido.";
                MessageColor = "Coral";
                return false;
            }
        }

        private void SetIPv4Address()
        {
            if (SelectedInterface?.Name == null || SelectedIPv4Address == null || string.IsNullOrWhiteSpace(NewIPAddress))
            {
                ValidationMessage = "O IP não pode estar vazio ou nulo. Por favor, insira um IPv4 válido.";
                MessageColor = "Coral";
                return;
            }

            if (!ValidateIPv4Address(NewIPAddress))
            {
                return;
            }

            if (SelectedInterface.IPv4Addresses.Any(x => x.Address == NewIPAddress))
            {
                ValidationMessage = $"O IPv4 {NewIPAddress} já está configurado nesta interface.";
                MessageColor = "Blue";
                return;
            }

            var result = _networkConfigurationService.SetIPv4Address(NewIPAddress, SelectedInterface.Name);

            if (result)
            {
                var ipToUpdate = SelectedInterface.IPv4Addresses.FirstOrDefault(ip => ip.Address == SelectedIPv4Address.Address);

                if (ipToUpdate != null)
                {
                    ipToUpdate.Address = NewIPAddress;
                    SelectedIPv4Address.Address = NewIPAddress;

                    ValidationMessage = $"Endereço IPv4 {NewIPAddress} definido com sucesso!";
                    MessageColor = "Green";
                }
                else
                {
                    ValidationMessage = "Erro ao definir o IPv4. Verifique a configuração ou tente novamente.";
                    MessageColor = "Coral";
                }
            }
        }

        private void RemoveIPv4Address()
        {

            if (SelectedInterface != null && SelectedIPv4Address != null)
            {
                if (!ValidateIPv4Address(SelectedIPv4Address.Address)) return;

                var iPAddressFound = SelectedInterface.IPv4Addresses.FirstOrDefault(x => x.Address == SelectedIPv4Address.Address);

                if (iPAddressFound != null)
                {
                    if (_networkConfigurationService.RemoveIPv4Address(SelectedIPv4Address.Address, SelectedInterface.Name!))
                    {
                        SelectedInterface.IPv4Addresses.Remove(iPAddressFound);
                        ValidationMessage = $"Endereço IPv4 {SelectedIPv4Address.Address} removido com sucesso!";
                        MessageColor = "Green";
                    }
                    else
                    {
                        ValidationMessage = $"Falha ao remover o endereço IPv4 {SelectedIPv4Address.Address}. Tente novamente.";
                        MessageColor = "Coral";
                    }
                }
                else
                {
                    ValidationMessage = $"O IP {SelectedIPv4Address.Address} não está presente na lista de IPv4 desta interface.";
                    MessageColor = "Blue";
                }
            }
            else
            {
                ValidationMessage = "Por favor, selecione um IPv4 válido da lista para remover.";
                MessageColor = "Coral";
            }
        }

        private void AddIPv4Address()
        {
            if (SelectedInterface?.Name == null || string.IsNullOrWhiteSpace(NewIPAddress))
            {
                ValidationMessage = "O IP não pode estar vazio ou nulo. Por favor, insira um IP válido.";
                MessageColor = "Coral";
                return;
            }

            if (!ValidateIPv4Address(NewIPAddress))
            {
                return;
            }

            if (SelectedInterface.IPv4Addresses.Any(x => x.Address == NewIPAddress))
            {
                ValidationMessage = $"O IPv4 {NewIPAddress} já está configurado nesta interface.";
                MessageColor = "Blue";
                return;
            }

            if (_networkConfigurationService.AddIPv4Address(NewIPAddress, SelectedInterface.Name))
            {
                SelectedInterface.IPv4Addresses.Add(new Ipv4Model() { Address = NewIPAddress });

                ValidationMessage = $"Endereço IPv4 {NewIPAddress} adicionado com sucesso!";
                MessageColor = "Green";
            }
            else
            {
                ValidationMessage = "Erro ao adicionar o IPv4. Tente novamente.";
                MessageColor = "Coral";
            }
        }

        private void SetAutomaticIPv4Address()
        {
            if (SelectedInterface?.Name == null)
            {
                ValidationMessage = "Por favor, selecione uma interface de rede antes de configurar o IPv4 automático.";
                MessageColor = "Coral";
                return;
            }

            bool success = _networkConfigurationService.SetIPv4Automatic(SelectedInterface.Name);

            if (success)
            {
                string newIpAddress = _networkConfigurationService.GetCurrentIPv4Address(SelectedInterface.Name);

                SelectedInterface.IPv4Addresses.Clear();
                SelectedInterface.IPv4Addresses.Add(new Ipv4Model { Address = newIpAddress });

                ValidationMessage = $"IPv4 automático configurado com sucesso para {SelectedInterface.Name}.";
                MessageColor = "Green";
            }
            else
            {
                ValidationMessage = "Falha ao configurar o IPv4 automático. Tente novamente.";
                MessageColor = "Coral";
            }
        }

        private void SetGateway()
        {
            if (SelectedInterface?.Name == null || string.IsNullOrWhiteSpace(Gateway))
            {
                ValidationMessage = "Por favor, selecione uma interface e insira um Gateway válido.";
                MessageColor = "Coral";
                return;
            }

            var subnetMask = SelectedInterface.IPv4Addresses
                .Where(x => x.Address == NewIPAddress)
                .Select(x => x.SubnetMask)
                .FirstOrDefault();

            if (!ValidateIPv4Address(NewIPAddress))
            {
                return;
            }

            var result = _networkConfigurationService.SetGateway(NewIPAddress, subnetMask, Gateway, SelectedInterface.Name);

            if (result)
            {
                SelectedInterface.Gateway = Gateway;
                ValidationMessage = $"Gateway {Gateway} configurado com sucesso!";
                MessageColor = "Green";
            }
            else
            {
                ValidationMessage = "Erro ao configurar o gateway. Verifique os dados e tente novamente.";
                MessageColor = "Coral";
            }
        }

        private void SetAutomaticGateway()
        {
            if (SelectedInterface?.Name == null)
            {
                ValidationMessage = "Por favor, selecione uma interface antes de configurar o Gateway automático.";
                MessageColor = "Coral";
                return;
            }

            string oldIpAddress = _networkConfigurationService.GetCurrentIPv4Address(SelectedInterface.Name);

            if (_networkConfigurationService.SetGatewayAutomatic(SelectedInterface.Name))
            {
                var ipToUpdate = SelectedInterface.IPv4Addresses.FirstOrDefault(ip => ip.Address == oldIpAddress);

                string currentIpAddress = _networkConfigurationService.GetCurrentIPv4Address(SelectedInterface.Name);

                if (ipToUpdate != null)
                {
                    ipToUpdate.Address = currentIpAddress;

                    if (SelectedIPv4Address != null)
                    {
                        SelectedIPv4Address.Address = currentIpAddress;
                    }
                }

                SelectedInterface.Gateway = string.Empty;
                ValidationMessage = $"Gateway automático configurado com sucesso para {SelectedInterface.Name}.";
                MessageColor = "Green";
            }
            else
            {
                ValidationMessage = "Falha ao configurar o Gateway automático. Tente novamente.";
                MessageColor = "Coral";
            }
        }

        private void SetDNS()
        {
            if (SelectedInterface?.Name == null || string.IsNullOrWhiteSpace(DNS))
            {
                ValidationMessage = "A interface tem que estar selecionada e o DNS válido. Por favor, confira.";
                MessageColor = "Coral";
                return;
            }

            var result = _networkConfigurationService.SetDNS(DNS, SelectedInterface.Name);

            if (result)
            {
                ValidationMessage = $"DNS {DNS} configurado com sucesso!";
                MessageColor = "Green";
            }
            else
            {
                ValidationMessage = "Erro ao configurar o DNS. Verifique se o endereço do DNS é válido e tente novamente.";
                MessageColor = "Coral";
            }
        }

        private void SetAutomaticDNS()
        {
            if (SelectedInterface?.Name == null)
            {
                ValidationMessage = "Por favor, selecione uma interface de rede antes de configurar o DNS automático.";
                MessageColor = "Coral";
                return;
            }

            if (_networkConfigurationService.SetDNSAutomatic(SelectedInterface.Name))
            {
                SelectedInterface.DNS = string.Empty;
                ValidationMessage = $"DNS automático configurado com sucesso para {SelectedInterface.Name}.";
                MessageColor = "Green";
            }
            else
            {
                ValidationMessage = "Falha ao configurar o DNS automático. Tente novamente.";
                MessageColor = "Coral";
            }
        }
        #endregion
    }
}
