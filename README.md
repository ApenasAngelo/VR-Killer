# VR Killer

O VR Killer é um programa simples projetado para renomear a pasta do OpenVR, o que resulta na incapacidade de funcionamento de jogos e programas que dependem dessa implementação. Ele foi desenvolvido com o objetivo de facilitar a solução alternativa de um bug em programas que abrem o runtime do SteamVR e dependem do mesmo aberto para funcionar, consumindo recursos do computador.
Foi desenvolvido em C# utilizando Windows Forms e WPF, com intuito de aprendizado da tecnologia.

**Nota:** Apesar de ter um caso de uso extremamente específico, pode ser de utilidade para alguém que vê nessa função uma solução para outro problema ;).

## Funcionamento

O VR Killer é um programa leve que vive na bandeja do Windows. Automaticamente, ele localiza a pasta padrão do `openvr`, mas pode ser manualmente selecionado nas configurações. Ele também permite a inicialização junto com o computador. Para utilizado, basta clicar duas vezes no seu ícone na bandeja. Alternativamente, clicar com o botão direito e clicar na primeira opção do menu terá o mesmo resultado, indicando a ação que será feita (Ativar/Desativar).