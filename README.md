Network Manager App

📌 Visão Geral
Aplicação desktop desenvolvida com Avalonia UI para a gestão de interfaces de rede, permitindo a configuração de endereços IP, gateway e servidores DNS.

🎯 Funcionalidades
✔ Listar Interfaces de Rede com informações detalhadas: nome da interface, tipo, estado, endereço MAC, entre outros.
✔ Selecionar e Configurar Interfaces de Rede: permite escolher uma interface específica e gerir as suas configurações de rede.
✔ Gestão de Endereços IP: listagem dos endereços e funcionalidades para adicionar, remover e modificar IPv4s configurados.
Funcionalidades para adicionar, remover e modificar IPv4. Opção para configuração automática.
✔ Outras definições de Rede: configuração manual ou automática do Gateway. Definição manual ou automática de Servidores DNS.

🛠 Tecnologias Utilizadas
C# com .NET
Avalonia UI para desenvolvimento multiplataforma e de aplicações desktop
Padrão MVVM como base para arquitetura
ReactiveUI

⚠️ Requisitos
Avalonia UI framework
Executar como Administrador: Algumas funcionalidades exigem permissões elevadas para modificar definições de rede.

🔄 Melhorias possíveis
⚡ Interface mais user-friendly: Melhor usabilidade dos botões para tornar a experiência mais intuitiva e feedback visual.
⚡ Desempenho com Programação Assíncrona: Implementação de async/await para evitar bloqueios na UI ao executar operações demoradas (exemplo: alterações de IP e DNS). Gestão eficiente de threads para reduzir delays e melhorar a resposta da aplicação.
⚡ Otimização do código e estrutura lógica: código mais modular para facilitar manutenção e adaptação de lógica.
