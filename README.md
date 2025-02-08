**Network Manager App**

📌 **Visão Geral**
<br>Aplicação desktop desenvolvida com Avalonia UI para a gestão de interfaces de rede, permitindo a configuração de endereços IP, gateway e servidores DNS.

🎯 **Funcionalidades**
<br>✔ Listar Interfaces de Rede e detalhes: nome da interface, tipo, estado, endereço MAC, entre outros.
<br>✔ Selecionar e configurar: permite escolher uma interface específica e gerir as suas configurações de rede.
<br>✔ Gestão de endereços IP: listagem dos endereços e funcionalidades para adicionar, remover e modificar IPv4s configurados.
Funcionalidades para adicionar, remover e modificar IPv4. Opção para configuração automática.
<br>✔ Outras definições de Rede: configuração manual ou automática do Gateway. Definição manual ou automática do DNS.

🛠 **Tecnologias Utilizadas**
<br>-C# com .NET
<br>-Avalonia UI para desenvolvimento multiplataforma e de aplicações desktop
<br>-Padrão MVVM como base para arquitetura
<br>-ReactiveUI

⚠️ **Requisitos**
<br>-.NET
<br>-Avalonia UI framework
<br>-Executar como Administrador: permissões para modificar definições de rede.

🔄 **Melhorias possíveis**
<br>-Interface user-friendly: usabilidade dos botões e interações mais intuitivas com feedback visual.
<br>-Desempenho com Programação Assíncrona: async/await para evitar bloqueios na UI ao executar operações demoradas. Gestão eficiente de threads para reduzir delays e melhorar a resposta da aplicação.
<br>-Loading e Feedback: indicador de carregamento durante operações.
<br>-Otimização do código e estrutura lógica: código modular e adaptação de lógica.
<br>-Error Handling e Logs: tratamento de erros e registo de logs para diagnósticos.
