<p align="center">
  <img src="assets/chip-source.png" alt="Chip" width="40%" align="middle">
  <img src="assets/capri.png" alt="Capri" width="40%" align="middle">
</p>

<h1 align="center">Capri Desktop · PT-BR</h1>

<p align="center">
  A Capri de <i>Cyberpunk: Edgerunners 2</i> morando no seu desktop, <b>em português do Brasil</b>.
</p>

<p align="center">
  Versão em português por <b>R3DPR0XY</b>, baseada no <a href="https://github.com/lachydotmcg/capri-desktop">capri-desktop</a> de <b>Lachy</b>.<br>
  Projeto de fã, não oficial e sem fins comerciais.
</p>

<table align="center">
  <tr>
    <td align="center"><img src="assets/readme/idle.webp" alt="Capri parada, piscando" width="150"><br><sub>De boa</sub></td>
    <td align="center"><img src="assets/readme/talk.webp" alt="Capri falando" width="150"><br><sub>Falando</sub></td>
    <td align="center"><img src="assets/readme/tossed.webp" alt="Capri sendo arremessada" width="150"><br><sub>Sendo arremessada</sub></td>
    <td align="center"><img src="assets/readme/glitch.webp" alt="Capri dando glitch" width="150"><br><sub>Glitch</sub></td>
    <td align="center"><img src="assets/readme/sleep.webp" alt="Capri dormindo e religando" width="150"><br><sub>Cochilando</sub></td>
  </tr>
</table>

## O que esta versão muda

- **Tudo em português:** janela de configurações (Mainframe), menu da bandeja, avisos, balões de fala, painel de tarefas e instalador.
- **Ela responde em português** desde a primeira vez que abre. Se você escolher outro idioma no Mainframe, a escolha fica salva.
- **Voz grátis em português:** usa a voz pt-BR do próprio Windows (por exemplo, a Microsoft Maria), que roda offline e não precisa baixar o pacote de voz de 300 MB. A voz grátis original foi treinada em inglês e chinês e soa estranha lendo português. Com uma chave da ElevenLabs, ela também fala português.
- **Atualizações vêm deste repositório**, então a atualização automática não troca a sua Capri em português pela versão em inglês.

O resto é a Capri do Lachy, com tudo o que ela faz: conversa, fala, abre apps, toca música, comenta seus jogos e senta nas suas janelas.

## O que ela faz

- **Conversa:** você digita e ela responde do jeito dela.
- **Fala em voz alta:** com a voz grátis do Windows ou com uma chave da ElevenLabs. Também tem chat por voz.
- **Faz coisas por você:** abre apps, toca música, fecha abas. É só pedir: "abre o Spotify", "toca Never Fade Away", "pausa", "o que está tocando?", "fecha o Discord".
- **Assiste você jogar:** se você ligar, ela comenta seus jogos.
- **Senta nas suas coisas:** solte ela numa janela, num ícone ou na parte de baixo da tela.

## Do que você precisa

- Windows 10 ou 11 (64 bits).
- Um microfone, se quiser conversar por voz.

| | Cérebro no seu PC | Cérebro por chave (como a NVIDIA grátis) |
| --- | --- | --- |
| Memória RAM | 8 GB (16 GB é melhor) | 4 GB |
| Placa de vídeo | Opcional; 4 GB ou mais deixa ela mais rápida | Não precisa |
| Espaço | 3 a 5 GB | Menos de 100 MB |
| Internet | Só para baixar | Sempre |

## Como instalar

1. Baixe o **Capri-Setup.exe** na aba [Releases](https://github.com/R3DPR0XY/Capri-desktop-PTBR/releases) e execute.
2. No Capri Mainframe, clique em **Montar o cérebro**, ou em **Chave grátis** para usar uma chave grátis da NVIDIA.
3. Opcional: cole uma chave da ElevenLabs para uma voz ainda melhor. Crie a chave com **Restrict key** desligado.

O Windows ou o antivírus podem avisar porque o app é novo e não tem assinatura digital. O código está todo aqui, e você pode compilar você mesmo (veja abaixo).

### Voz em português

- **Para ela falar:** a voz pt-BR do Windows normalmente já vem instalada quando o Windows está em português. Se não estiver, instale em **Configurações > Hora e idioma > Fala > Gerenciar vozes**.
- **Para ela te ouvir:** o chat por voz grátis usa o reconhecimento de fala do Windows. Ative o **Reconhecimento de fala online** em **Configurações > Privacidade e segurança > Fala**. Os "ouvidos offline" da versão original só entendem inglês.

## Controles

| Controle | O que faz |
| --- | --- |
| Lápis | digitar para ela |
| Onda | chat por voz |
| Arrastar | sentar ela numa janela |
| Shift + soltar | senta exatamente onde você soltou |
| Ctrl + soltar | ela fica pendurada; clique nela para soltar |
| Ctrl + Alt + V | liga ou desliga o chat por voz |
| Ctrl + Alt + N | muta o seu microfone |
| Ctrl + Alt + T | abre a caixa de digitar de qualquer lugar |
| Ctrl + Alt + H | mostra ou esconde ela |
| Esc | para uma tarefa que ela está fazendo |
| Ctrl + Alt + C | tira ela do seu cursor |
| Clique direito nela ou no ícone da bandeja | menu e configurações |

## Segurança

Antes de traduzir, o código original foi auditado. Os pontos principais:

- **Sem vírus e sem envio escondido.** O app só fala com o provedor de IA que você escolher, com a ElevenLabs (se você ligar) e com o GitHub para atualizações.
- **Downloads conferidos:** atualizações, modelos de IA e pacotes de voz são verificados por checksum SHA-256 antes de rodar.
- **Chaves de API** ficam criptografadas com o DPAPI do Windows.
- **Teclado:** a única tecla que ela monitora é o **Esc**, para você parar uma tarefa.
- **"Ignorar permissões"** (em HÁBITOS) vem **desligado**. Com ele ligado, ela compra, envia, apaga e instala sem perguntar. Deixe desligado.
- Quando ela usa o computador por você, prints da tela vão para a IA escolhida.

## Compilar você mesmo

Só precisa do que já vem no Windows e do Windows SDK:

```powershell
./ci/build-windows.ps1
```

O app e o instalador saem em `out/windows`. Para rodar também os autotestes do projeto:

```powershell
./ci/build-windows.ps1 -SmokeTest
```

Cada envio para este repositório é compilado e testado automaticamente no GitHub Actions.

## Créditos e licença

- **Capri Desktop original:** [Lachy (lachydotmcg)](https://github.com/lachydotmcg/capri-desktop). O README original está em [README.en.md](README.en.md).
- **Tradução e adaptação para PT-BR:** R3DPR0XY.

Este projeto segue a mesma licença do original, **PolyForm Noncommercial 1.0.0** ([LICENSE.md](LICENSE.md)). Você pode usar, modificar e compartilhar **sem fins comerciais**, mantendo a licença e o aviso abaixo:

> Required Notice: Copyright 2026 Lachy (https://github.com/lachydotmcg)

A Capri, o nome e o visual dela pertencem à CD PROJEKT RED, Studio Trigger e Netflix. Este é um projeto de fã sem vínculo oficial.
