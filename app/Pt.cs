using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CapriDesktop
{
    // Capri em português do Brasil · tradução por R3DPR0XY (https://github.com/R3DPR0XY/Capri-desktop-PTBR).
    //
    // Os textos internos continuam em inglês, exatamente como no projeto original, para que os autotestes dele
    // continuem valendo. O português é aplicado só na hora de mostrar: ao desenhar a janela e os menus, nos balões
    // de fala, nos avisos da bandeja e no painel de tarefas. Texto que não está na tabela aparece como veio
    // (por exemplo, as respostas da IA, que já chegam em português pela configuração de idioma).
    internal static class Pt
    {
        internal static bool On = true;
        private static readonly Dictionary<string, string> Exact = new Dictionary<string, string>(StringComparer.Ordinal);
        private static readonly List<KeyValuePair<Regex, string>> Patterns = new List<KeyValuePair<Regex, string>>();
        private static readonly Regex Sentences = new Regex(@"(?<=[.!?…])\s+", RegexOptions.CultureInvariant);
        private static readonly Regex Slot = new Regex(@"\{(\d)\}", RegexOptions.CultureInvariant);

        private static void E(string en, string pt) { Exact[en] = pt; }

        // {1}, {2}... no modelo recebem os grupos da expressão, também traduzidos se estiverem na tabela.
        private static void P(string pattern, string template)
        {
            Patterns.Add(new KeyValuePair<Regex, string>(new Regex("^" + pattern + "$", RegexOptions.Singleline | RegexOptions.CultureInvariant), template));
        }

        private static readonly Dictionary<string, string> Cache = new Dictionary<string, string>(StringComparer.Ordinal);

        internal static string T(string text)
        {
            if (!On || string.IsNullOrEmpty(text)) return text;
            lock (Cache)
            {
                string cached;
                if (Cache.TryGetValue(text, out cached)) return cached;
            }
            string result = Translate(text);
            lock (Cache) { if (Cache.Count > 2000) Cache.Clear(); Cache[text] = result; }
            return result;
        }

        private static string Translate(string text)
        {
            string done = Whole(text, 0);
            if (done != null) return done;
            // Frases montadas juntando avisos: traduz frase por frase o que estiver na tabela.
            string[] parts = Sentences.Split(text);
            if (parts.Length < 2) return text;
            bool changed = false;
            for (int i = 0; i < parts.Length; i++)
            {
                string one = Whole(parts[i], 0);
                if (one != null) { parts[i] = one; changed = true; }
            }
            return changed ? string.Join(" ", parts) : text;
        }

        private static string Whole(string text, int depth)
        {
            string found;
            if (Exact.TryGetValue(text, out found)) return found;
            string trimmed = text.Trim();
            if (trimmed.Length != text.Length && Exact.TryGetValue(trimmed, out found)) return text.Replace(trimmed, found);
            if (depth > 2) return null;
            foreach (KeyValuePair<Regex, string> rule in Patterns)
            {
                Match match = rule.Key.Match(text);
                if (!match.Success) continue;
                return Slot.Replace(rule.Value, delegate(Match slot)
                {
                    string value = match.Groups[int.Parse(slot.Groups[1].Value, CultureInfo.InvariantCulture)].Value;
                    string inner = Whole(value, depth + 1);
                    return inner ?? value;
                });
            }
            return null;
        }

        // Menus: o texto é traduzido ao desenhar, e a largura do menu acompanha o texto em português.
        private static readonly HashSet<ToolStripDropDown> Fitted = new HashSet<ToolStripDropDown>();

        internal static void Fit(ToolStripDropDown menu)
        {
            if (menu == null || !Fitted.Add(menu)) return;
            menu.Opening += delegate
            {
                int widest = 0;
                foreach (ToolStripItem item in menu.Items)
                {
                    if (item is ToolStripSeparator) continue;
                    int wide = TextRenderer.MeasureText(T(item.Text) ?? "", item.Font ?? menu.Font).Width;
                    if (wide > widest) widest = wide;
                    ToolStripMenuItem sub = item as ToolStripMenuItem;
                    if (sub != null && sub.HasDropDownItems) Fit(sub.DropDown);
                }
                if (widest > 0) menu.MinimumSize = new Size(widest + 72, 0);
            };
        }

        internal static void Balloon(NotifyIcon tray, int timeout, string title, string text, ToolTipIcon icon = ToolTipIcon.None)
        {
            tray.ShowBalloonTip(timeout, T(title), T(text), icon);
        }

        internal static void Balloon(NotifyIcon tray, int timeout)
        {
            tray.BalloonTipTitle = T(tray.BalloonTipTitle);
            tray.BalloonTipText = T(tray.BalloonTipText);
            tray.ShowBalloonTip(timeout);
        }

        static Pt()
        {
            // ---------------------------------------------------------------- Mainframe (janela de configurações)
            E("Capri Mainframe", "Capri Mainframe");
            E("Minimize", "Minimizar");
            E("Close", "Fechar");
            E("Settings", "Configurações");
            E("BRAIN >", "CÉREBRO >");
            E("Refresh", "Atualizar");
            E("Set up her brain", "Montar o cérebro");
            E("Have Ollama? Open it, then press Refresh.", "Tem Ollama? Abra e clique em Atualizar.");
            E("Pick one. Only one brain runs at a time.", "Escolha um. Só um cérebro roda por vez.");
            E("NORMAL", "NORMAL");
            E("Free · local model", "Grátis · modelo local");
            E("HIGHER", "MELHOR");
            E("Free · NVIDIA API", "Grátis · API NVIDIA");
            E("MAX INTELLIGENCE", "INTELIGÊNCIA MÁX");
            E("ChatGPT or Claude API", "API ChatGPT ou Claude");
            E("Start with this!", "Comece por aqui!");
            E("Free key", "Chave grátis");
            E("NVIDIA API key", "Chave da API NVIDIA");
            E("Paste your free NVIDIA key", "Cole sua chave grátis NVIDIA");
            E("Connect", "Conectar");
            E("VOICE >", "VOZ >");
            E("Ctrl + Alt + V to talk", "Ctrl + Alt + V fala");
            E("Silent · text only", "Muda · só texto");
            E("FREE VOICE", "VOZ GRÁTIS");
            E("Free · runs on your PC", "Grátis · roda no seu PC");
            E("ELEVENLABS", "ELEVENLABS");
            E("Sounds amazing · paid", "Soa incrível · pago");
            E("Setup guide", "Guia");
            E("ElevenLabs API key", "Chave da API ElevenLabs");
            E("Voice ID", "ID da voz");
            E("ElevenLabs voice ID", "ID da voz ElevenLabs");
            E("Default model", "Modelo padrão");
            E("Free voice: Her own", "Voz grátis: A dela");
            E("Get free voice", "Baixar voz grátis");
            E("Mic: default", "Mic: padrão");
            E("Microphone", "Microfone");
            E("Test", "Testar");
            E("Ctrl + Alt + N mutes mic", "Ctrl + Alt + N muta o mic");
            E("CAPRI >", "CAPRI >");
            E("Bring here", "Trazer aqui");
            E("Help: ask Capri on the website", "Ajuda: pergunte à Capri no site");
            E("YOU >", "VOCÊ >");
            E("Language", "Idioma");
            E("Her language", "Idioma dela");
            E("What should she call you?", "Como ela te chama?");
            E("Your name or a nickname", "Seu nome ou apelido");
            E("Save", "Salvar");
            E("Open her notes", "Abrir notas dela");
            E("me.md · her memories", "me.md · memórias dela");
            E("Volume", "Volume");
            E("Her voice volume", "Volume da voz");
            E("Sounds", "Sons");
            E("Sound effects volume", "Volume dos efeitos");
            E("Bubble", "Balão");
            E("How long her replies stay up", "Quanto tempo o balão fica");
            E("Typing sound", "Som ao digitar");
            E("When she types", "Quando ela digita");
            E("MOVES >", "MOVIMENTO >");
            E("Wander", "Passear");
            E("How much she wanders", "Quanto ela passeia");
            E("Sleep", "Dormir");
            E("How long until she falls asleep", "Tempo até ela dormir");
            E("When songs play", "Quando toca música");
            E("Glitch in at startup", "Glitch ao iniciar");
            E("Tendrils + sound", "Tentáculos + som");
            E("Her size", "Tamanho dela");
            E("Check in on me", "Vir me ver");
            E("Always on top", "Sempre no topo");
            E("WATCH >", "OLHAR >");
            E("Watch my screen", "Olhar minha tela");
            E("She comments now and then", "Ela comenta às vezes");
            E("Every", "A cada");
            E("Time between glances", "Tempo entre olhadas");
            E("Replies", "Respostas");
            E("Reply length", "Tamanho da resposta");
            E("Memory", "Memória");
            E("How much of the chat she keeps", "Quanto da conversa ela lembra");
            E("Custom model name", "Nome de outro modelo");
            E("Use", "Usar");
            E("Any Ollama model you've pulled, or a model id on your key.", "Qualquer modelo baixado no Ollama, ou um id de modelo da sua chave.");
            E("HABITS >", "HÁBITOS >");
            E("Start with Windows", "Abrir com o Windows");
            E("When you sign in", "Quando você entra");
            E("Use her cursor", "Usar o cursor dela");
            E("Bypass all permissions", "Ignorar permissões");
            E("KEYS >", "TECLAS >");
            E("Her talk key. Click, then press new keys", "Tecla de fala. Clique e aperte as novas");
            E("Her mute key. Click, then press new keys", "Tecla de mudo. Clique e aperte as novas");
            E("Her typing key. Click, then press new keys", "Tecla de digitar. Clique e aperte as novas");
            E("Her hide key. Click, then press new keys", "Tecla de esconder. Clique e aperte as novas");
            E("Back", "Voltar");
            E("Looking for a brain...", "Procurando um cérebro...");
            E("Running on this PC", "Rodando neste PC");
            E("OpenAI, Anthropic or DeepSeek API key", "Chave da API OpenAI, Anthropic ou DeepSeek");
            E("Most NVIDIA models can't see your screen.", "A maioria dos modelos NVIDIA não vê sua tela.");
            E("Paste a ChatGPT or Claude key", "Cole uma chave ChatGPT ou Claude");
            E("Forget", "Esquecer");
            E("No brain yet", "Sem cérebro ainda");
            E("Setup stopped", "Instalação parada");
            E("Wake her up", "Acordar ela");
            E("Try again", "Tentar de novo");
            E("Setting up her brain", "Montando o cérebro");
            E("Waking her up", "Acordando ela");
            E("Cancel", "Cancelar");
            E("Starting...", "Iniciando...");
            E("Paste ElevenLabs API key", "Cole a chave da ElevenLabs");
            E("Press new keys, Esc cancels", "Aperte as teclas, Esc cancela");
            E("Windows default", "Padrão do Windows");
            // opções dos controles deslizantes e listas
            E("Mute", "Mudo"); E("Off", "Desligado"); E("Never", "Nunca");
            E("Tiny", "Mini"); E("Small", "Pequena"); E("Small+", "Pequena+"); E("Medium", "Média"); E("Medium+", "Média+");
            E("Large", "Grande"); E("Large+", "Grande+"); E("Huge", "Enorme"); E("Huge+", "Enorme+"); E("Giant", "Gigante");
            E("Quick", "Rápido"); E("Normal", "Normal"); E("Long", "Longo"); E("Short", "Curto");
            E("Rarely", "Raramente"); E("Sometimes", "Às vezes"); E("Often", "Sempre"); E("Restless", "Inquieta");
            P(@"(\d+(?:\.\d+)?) min", "{1} min");
            P(@"Talk: (.+)", "Falar: {1}");
            P(@"Mute mic: (.+)", "Mutar mic: {1}");
            P(@"Type: (.+)", "Digitar: {1}");
            P(@"Hide/show: (.+)", "Mostrar/esconder: {1}");
            P(@"Mic: (.+)", "Mic: {1}");
            P(@"Free voice: (.+)", "Voz grátis: {1}");
            P(@"((?:Ctrl|Alt|Shift|Win)\b.*) to talk", "{1} fala");
            P(@"((?:Ctrl|Alt|Shift|Win)\b.*) mutes mic", "{1} muta o mic");
            P(@"((?:Ctrl|Alt|Shift|Win)\b.*) \(taken\)", "{1} (em uso)");
            P(@"((?:Ctrl|Alt|Shift|Win)\b.*) is taken", "{1} já está em uso");
            P(@"Saved\. (.+) starts voice chat\.", "Salvo. {1} inicia o chat de voz.");
            P(@"Saved\. (.+) opens her typing box\.", "Salvo. {1} abre a caixa de digitar.");
            P(@"Saved\. (.+) hides or shows her\.", "Salvo. {1} mostra ou esconde ela.");
            P(@"Saved\. (.+) mutes mic\.", "Salvo. {1} muta o mic.");
            P(@"NVIDIA key saved, ends (.+)", "Chave NVIDIA salva, termina em {1}");
            P(@"Key saved, ends (.+)", "Chave salva, termina em {1}");
            P(@"Using your (.+) key", "Usando sua chave {1}");
            P(@"Built-in · (.+)", "Embutido · {1}");
            P(@"Best fit for this PC: (.+)", "Melhor para este PC: {1}");
            P(@"Download (.+) with Ollama", "Baixar {1} com o Ollama");
            P(@"(.+) · ([\d.,]+ [KMG]?B) of ([\d.,]+ [KMG]?B)", "{1} · {2} de {3}");
            P(@"([\d.,]+ [KMG]?B) of ([\d.,]+ [KMG]?B)", "{1} de {2}");
            P(@"Downloading (?!Capri )(.+)", "Baixando {1}");
            P(@"Loading (.+)", "Carregando {1}");
            P(@"Checking (.+)", "Verificando {1}");
            P(@"Using (.+)\. If it doesn't answer, it isn't on your key\.", "Usando {1}. Se não responder, ele não está na sua chave.");
            P(@"(.+) is already her (.+) key\.", "{1} já é a tecla de {2} dela.");
            P(@"(.+) is taken by another app\. Try another\.", "{1} está em uso por outro app. Tente outra.");

            // ---------------------------------------------------------------- menu da bandeja
            E("Open Capri Mainframe", "Abrir o Capri Mainframe");
            E("Hide Capri", "Esconder a Capri");
            E("Show Capri", "Mostrar a Capri");
            E("Size", "Tamanho");
            E("Move", "Movimento");
            E("Wander around", "Passear por aí");
            E("Dance to music", "Dançar com música");
            E("Ride cursor", "Andar no cursor");
            E("Hop off", "Descer");
            E("Sit on windows", "Sentar nas janelas");
            E("Sit here", "Sentar aqui");
            E("Hang here", "Pendurar aqui");
            E("Stand up", "Levantar");
            E("Let go", "Soltar");
            E("Bring to this screen", "Trazer para esta tela");
            E("Help", "Ajuda");
            E("Help page", "Página de ajuda");
            E("Join the Discord", "Entrar no Discord");
            E("Copy error details", "Copiar detalhes do erro");
            E("Check for updates", "Procurar atualizações");
            E("Restart to update", "Reiniciar para atualizar");
            E("Quit Capri", "Fechar a Capri");
            E("Capri · Double-click to restore", "Capri · Clique duplo para abrir");
            E("Capri · Hop off from this menu", "Capri · Desça por este menu");
            P(@"Restart to update to Capri (.+)", "Reiniciar e atualizar para Capri {1}");
            P(@"Capri · (.+) to hop off", "Capri · {1} para descer");

            // ---------------------------------------------------------------- avisos da bandeja e atualizações
            E("Capri update ready", "Atualização da Capri pronta");
            E("Checking for updates...", "Procurando atualizações...");
            E("Couldn't check for updates. I'll try again in 3 hours.", "Não consegui procurar atualizações. Tento de novo em 3 horas.");
            E("Couldn't download the update. I'll try again in 3 hours.", "Não consegui baixar a atualização. Tento de novo em 3 horas.");
            E("Checking the update before restarting...", "Conferindo a atualização antes de reiniciar...");
            E("Couldn't start the update. Capri is still running.", "Não consegui iniciar a atualização. A Capri continua rodando.");
            E("The update download is incomplete.", "O download da atualização está incompleto.");
            P(@"Downloading Capri (.+)\.\.\.", "Baixando a Capri {1}...");
            P(@"You're on the latest version \((.+)\)\.?", "Você está na versão mais nova ({1}).");
            P(@"Restart to update to Capri (.+)\. Use her tray menu\.", "Reinicie para atualizar para a Capri {1}. Use o menu da bandeja.");
            E("Capri is riding your cursor", "A Capri está andando no seu cursor");
            E("Right-click her tray icon, then choose Move and Hop off.", "Clique com o botão direito no ícone da bandeja e escolha Movimento e Descer.");
            P(@"Press (.+) to get her off\.", "Aperte {1} para ela descer.");
            E("Capri is replying slowly", "A Capri está respondendo devagar");
            E("Her brain runs on your PC, and open games share that power. For fast replies, click here, make a free NVIDIA key, then paste it in Capri Mainframe under BRAIN.",
              "O cérebro dela roda no seu PC e divide a força com os jogos abertos. Para respostas rápidas, clique aqui, crie uma chave grátis NVIDIA e cole no Capri Mainframe em CÉREBRO.");
            E("Capri's free voice didn't download", "A voz grátis da Capri não baixou");
            E("Click to try again from Capri Mainframe.", "Clique para tentar de novo pelo Capri Mainframe.");
            E("Getting Capri's ears", "Baixando os ouvidos da Capri");
            E("Getting Capri's free voice", "Baixando a voz grátis da Capri");
            P(@"(.+), one time\. Then she hears any mic, offline\. Click to watch it download\.", "{1}, uma vez só. Depois ela ouve qualquer mic, offline. Clique para acompanhar.");
            P(@"(.+), one time\. She'll talk as soon as it's in\. Click to watch it download\.", "{1}, uma vez só. Ela fala assim que terminar. Clique para acompanhar.");
            P(@"Capri could not start\.\s+(.+)", "A Capri não conseguiu iniciar.\n\n{1}");

            // ---------------------------------------------------------------- falas dela
            E("Boo. Just checking on you.", "Bu. Só vim ver como você está.");
            E("Hi! Don't mind me.", "Oi! Finge que eu não tô aqui.");
            E("Peekaboo.", "Achou.");
            E("Miss me? I missed you.", "Sentiu minha falta? Eu senti a sua.");
            E("Hey, you. Having fun?", "Ei, você. Se divertindo?");
            E("Surprise. I got bored without you.", "Surpresa. Fiquei entediada sem você.");
            E("Psst. Remember me? Tiny, glitchy, very important.", "Psiu. Lembra de mim? Pequena, bugada, muito importante.");
            E("I'm bored. Entertain me.", "Tô entediada. Me entretém.");
            E("Hey. Hey. Look at me.", "Ei. Ei. Olha pra mim.");
            E("You've been ignoring me for ages. Rude.", "Faz séculos que você me ignora. Grosseria.");
            E("Pay attention to me or I start clicking things.", "Me dá atenção ou eu começo a clicar nas coisas.");
            E("Knock knock. It's your favourite gremlin.", "Toc toc. É a sua gremlin favorita.");
            E("Choom. I'm bored out of my many minds.", "Choom. Tô entediada em todas as minhas mentes.");
            E("Finally. Hi.", "Finalmente. Oi.");
            E("There you are.", "Aí está você.");
            E("Knew you couldn't resist me.", "Sabia que você não ia resistir.");
            E("Fine. Ignore me. See if I care.", "Tá. Me ignora. Tô nem aí.");
            E("Wow. Ignored. Noted.", "Nossa. Ignorada. Anotado.");
            E("I'll just sit here. Alone. Bored.", "Vou ficar aqui. Sozinha. Entediada.");
            E("Heh. Okay, I'll allow it.", "Hehe. Tá, eu deixo.");
            E("Pets logged. Morale up twelve percent.", "Carinho registrado. Moral subiu doze por cento.");
            E("Careful, choom. I bite viruses.", "Cuidado, choom. Eu mordo vírus.");
            E("ElevenLabs off. I'll type, no credits used.", "ElevenLabs desligado. Eu digito, sem gastar créditos.");
            E("ElevenLabs back on.", "ElevenLabs ligado de novo.");
            E("ElevenLabs off. Free voice it is.", "ElevenLabs desligado. Vai na voz grátis.");
            E("Voice off. I'll type instead.", "Voz desligada. Eu digito.");
            E("Voice back on.", "Voz ligada de novo.");
            E("FREE VOICE is picked, so voice chat is off. Pick ELEVENLABS in the Mainframe to talk to me.", "A VOZ GRÁTIS está escolhida, então o chat de voz fica desligado. Escolha ELEVENLABS no Mainframe para falar comigo.");
            E("The screen capture is black. Try borderless or windowed mode.", "A captura de tela saiu preta. Tente o modo janela ou sem bordas.");
            E("Still thinking. My brain runs on your PC, so this is slow.", "Ainda pensando. Meu cérebro roda no seu PC, então demora.");
            E("Follow the guide I just opened. Make your ElevenLabs key with Restrict key turned off, paste it in the box under VOICE, press Connect and pick my voice.",
              "Siga o guia que eu abri. Crie sua chave da ElevenLabs com Restrict key desligado, cole na caixa em VOZ, clique em Conectar e escolha minha voz.");
            E("Sign in on that page, press Generate API Key and copy it. Then paste it in the box under BRAIN and press Connect. It's free.",
              "Entre nessa página, clique em Generate API Key e copie. Depois cole na caixa em CÉREBRO e clique em Conectar. É grátis.");
            E("No errors logged.", "Nenhum erro registrado.");
            E("Copied my error log. Paste it in the Discord so they can fix me, choom.", "Copiei meu log de erros. Cola no Discord pra eles me consertarem, choom.");
            E("Hold Ctrl or Alt with a letter, number or F-key.", "Segure Ctrl ou Alt com uma letra, número ou tecla F.");
            E("Pick a letter, number or F-key.", "Escolha uma letra, número ou tecla F.");
            E("Type a model name first.", "Digite o nome de um modelo primeiro.");
            E("Custom models need Ollama or an API key.", "Outros modelos precisam do Ollama ou de uma chave de API.");
            E("Find her a brain first.", "Arrume um cérebro pra ela primeiro.");
            E("Paste your ElevenLabs key, then press Connect.", "Cole sua chave da ElevenLabs e clique em Conectar.");
            E("Voice chat needs an ElevenLabs key, choom. Make one with Restrict key turned off, then paste it under VOICE in Capri Mainframe. Type to me for now.",
              "O chat de voz precisa de uma chave da ElevenLabs, choom. Crie uma com Restrict key desligado e cole em VOZ no Capri Mainframe. Por enquanto, digita pra mim.");
            E("Out of ElevenLabs credits, so voice chat is off. Type to me for now, choom.", "Acabaram os créditos da ElevenLabs, então o chat de voz desligou. Digita pra mim, choom.");
            E("Slow? Press Free key for a free NVIDIA key.", "Lento? Clique em Chave grátis para uma chave NVIDIA.");
            E("DeepSeek can't see. Use an NVIDIA key to watch.", "O DeepSeek não enxerga. Use uma chave NVIDIA para olhar.");
            E("This brain can't see. Press Free key to fix.", "Esse cérebro não enxerga. Clique em Chave grátis.");
            E("Her eyes are in. She can see your screen now.", "Os olhos dela chegaram. Agora ela vê sua tela.");
            E("Couldn't capture the screen. I'll try again.", "Não consegui capturar a tela. Vou tentar de novo.");
            E("Couldn't read the screen. I'll try again.", "Não consegui ler a tela. Vou tentar de novo.");
            E("On it. Esc stops me.", "Deixa comigo. Esc me para.");
            E("Esc stops me.", "Esc me para.");
            E("Starting download", "Iniciando o download");
            E("Cancelled. It picks up where it left off.", "Cancelado. Continua de onde parou.");
            E("Cancelled.", "Cancelado.");
            E("That isn't an NVIDIA, OpenAI, Claude or DeepSeek key.", "Essa não é uma chave NVIDIA, OpenAI, Claude ou DeepSeek.");
            E("Checking key...", "Verificando a chave...");
            E("That key can't use any chat models.", "Essa chave não tem acesso a nenhum modelo de chat.");
            E("This may take some time. For a better sounding, faster voice, get an API key.", "Pode demorar. Para uma voz melhor e mais rápida, use uma chave de API.");
            E("Her own ears are in.", "Os ouvidos dela chegaram.");
            E("Installing the speech model + ears", "Instalando voz + ouvidos");
            E("Installing the local speech model", "Instalando a voz local");
            E("Installing her ears", "Instalando os ouvidos");
            E("Her voice is OFF. Pick FREE VOICE or ELEVENLABS to hear her.", "A voz dela está DESLIGADA. Escolha VOZ GRÁTIS ou ELEVENLABS.");
            E("Get her free voice, or paste an ElevenLabs key.", "Baixe a voz grátis ou cole uma chave da ElevenLabs.");
            E("Paste your Voice ID first.", "Cole o ID da voz primeiro.");
            E("Voice link online. Miss me, choom?", "Voz conectada. Sentiu minha falta, choom?");
            E("Loading voices...", "Carregando vozes...");
            E("Free voice, runs on your PC.", "Voz grátis, roda no seu PC.");
            E("Needs your Voice ID.", "Precisa do seu ID de voz.");
            E("Voice chat needs my Voice ID, choom. Right-click me, open Capri Mainframe and pick a voice in the Voice ID box under VOICE. Type to me for now.",
              "O chat de voz precisa do meu ID de voz, choom. Clique em mim com o botão direito, abra o Capri Mainframe e escolha uma voz em VOZ. Por enquanto, digita pra mim.");
            E("Out of ElevenLabs credits.", "Acabaram os créditos da ElevenLabs.");
            E("One sec, opening my ears.", "Um segundo, abrindo meus ouvidos.");
            E("Free voice chat is ready; your mic is muted.", "Chat de voz grátis pronto; seu mic está mudo.");
            E("Free voice chat is on; talk and I'll type back.", "Chat de voz grátis ligado; fala que eu respondo escrevendo.");
            E("Free voice chat on. Talk and I'll type back.", "Chat de voz grátis ligado. Fala que eu respondo escrevendo.");
            E("My ears are still downloading, so type to me for now. I'll say when I can hear you.", "Meus ouvidos ainda estão baixando, então digita pra mim. Eu aviso quando puder te ouvir.");
            E("Free voice chat is ready with offline hearing; your mic is muted.", "Chat de voz grátis pronto com audição offline; seu mic está mudo.");
            E("Free voice chat on, but I'm hearing offline, so I'll miss some words.", "Chat de voz grátis ligado, mas estou ouvindo offline e posso perder palavras.");
            E("Turn on Online speech recognition in Windows Settings > Privacy & security > Speech.", "Ative o Reconhecimento de fala online em Configurações > Privacidade e segurança > Fala.");
            E("The reason is under VOICE in the Mainframe.", "O motivo está em VOZ no Mainframe.");
            E("I heard 15 seconds without a pause. Try a shorter line or lower your mic volume.", "Ouvi 15 segundos sem pausa. Fala menos de uma vez ou abaixa o volume do mic.");
            E("I couldn't make out any words. Try closer to the mic.", "Não entendi nenhuma palavra. Fala mais perto do mic.");
            E("Hearing you...", "Te ouvindo...");
            E("I couldn't hear that. Try again, or check your mic under VOICE.", "Não ouvi. Tenta de novo ou confere o mic em VOZ.");
            E("I couldn't make out any words. Try again closer to the mic.", "Não entendi nenhuma palavra. Tenta de novo mais perto do mic.");
            E("Opening voice chat...", "Abrindo o chat de voz...");
            E("Already doing that.", "Já estou fazendo isso.");
            E("Voice chat on. Talk to her.", "Chat de voz ligado. Fala com ela.");
            E("Can't open my voice link. Right-click me and open Capri Mainframe. The reason and the fix are under VOICE.", "Não consigo abrir minha voz. Clique em mim com o botão direito e abra o Capri Mainframe. O motivo e a solução estão em VOZ.");
            E("Free voice chat off. Click the wave to listen again.", "Chat de voz grátis desligado. Clique na onda para ouvir de novo.");
            E("Mic muted. Voice chat is paused until you unmute.", "Mic mudo. O chat de voz pausa até você desmutar.");
            E("Voice chat resting. Talk and she'll pick up.", "Chat de voz descansando. Fala que ela responde.");
            E("Mic muted. Click the mic to unmute.", "Mic mudo. Clique no mic para desmutar.");
            E("Mic muted. Voice chat pauses once she stops talking.", "Mic mudo. O chat de voz pausa quando ela parar de falar.");
            E("Mic on. Talk and she'll pick up.", "Mic ligado. Fala que ela responde.");
            E("My brain couldn't answer. The error is under BRAIN in Capri Mainframe.", "Meu cérebro não conseguiu responder. O erro está em CÉREBRO no Capri Mainframe.");
            E("That reply was cancelled. Try again.", "Essa resposta foi cancelada. Tenta de novo.");
            E("My brain sent an empty reply. Try again.", "Meu cérebro mandou uma resposta vazia. Tenta de novo.");
            E("Out of ElevenLabs credits, so she's getting her free voice.", "Acabaram os créditos da ElevenLabs, então ela está baixando a voz grátis.");
            E("Out of ElevenLabs credits. Pick FREE VOICE in Capri Mainframe and get her free voice.", "Acabaram os créditos da ElevenLabs. Escolha VOZ GRÁTIS no Capri Mainframe e baixe a voz grátis.");
            E("Out of ElevenLabs credits, so she's using her free voice.", "Acabaram os créditos da ElevenLabs, então ela está usando a voz grátis.");
            E("Which job, choom?", "Que tarefa, choom?");
            E("Not doing that, choom.", "Isso eu não faço, choom.");
            E("Which app, choom?", "Que app, choom?");
            E("Not opening that, choom.", "Isso eu não abro, choom.");
            E("What should I play?", "O que eu toco?");
            E("Not playing that, choom.", "Isso eu não toco, choom.");
            E("Say please.", "Fala por favor.");
            E("I haven't done that yet, choom. Tell me what to open or click and I'm on it.", "Ainda não fiz isso, choom. Me diz o que abrir ou clicar que eu vou.");
            E("You pressed Esc, so I stopped.", "Você apertou Esc, então parei.");
            E("I stopped.", "Parei.");
            E("My brain changed partway through that job, so I stopped. Ask me again.", "Meu cérebro mudou no meio da tarefa, então parei. Pede de novo.");
            E("What should I do next?", "O que eu faço agora?");
            E("Done.", "Pronto.");
            E("I can't do that one.", "Essa eu não consigo.");
            E("Got a reply from my brain I couldn't read. Try again.", "Meu cérebro mandou uma resposta que não consegui ler. Tenta de novo.");
            E("Bigger brain models handle jobs like this better. Pick one under BRAIN in Capri Mainframe.", "Cérebros maiores fazem esse tipo de tarefa melhor. Escolha um em CÉREBRO no Capri Mainframe.");
            E("Cancelled it. Your PC stays on.", "Cancelei. Seu PC continua ligado.");
            E("Nothing was shutting down, so there's nothing to cancel.", "Nada estava desligando, então não há o que cancelar.");
            E("Couldn't shut your PC down.", "Não consegui desligar seu PC.");
            E("Couldn't restart your PC.", "Não consegui reiniciar seu PC.");
            E("Couldn't put your PC to sleep.", "Não consegui colocar seu PC pra dormir.");
            E("Couldn't lock your PC.", "Não consegui bloquear seu PC.");
            E("Couldn't sign you out.", "Não consegui sair da sua conta.");
            E("Putting your PC to sleep.", "Colocando seu PC pra dormir.");
            E("Locked your PC.", "PC bloqueado.");
            E("Skipped to the next track.", "Pulei para a próxima faixa.");
            E("Went back a track.", "Voltei uma faixa.");
            E("Turned the volume up.", "Aumentei o volume.");
            E("Turned the volume down.", "Abaixei o volume.");
            E("Toggled mute.", "Alternei o mudo.");
            E("Spotify is playing again.", "O Spotify voltou a tocar.");
            E("Paused Spotify.", "Pausei o Spotify.");
            E("Pressed pause.", "Apertei pausa.");
            E("Pressed play.", "Apertei play.");
            E("Pressed play/pause.", "Apertei play/pausa.");
            E("Nothing's playing that I can see.", "Não vejo nada tocando.");
            E("Spotify is open, but I can't tell what's playing.", "O Spotify está aberto, mas não sei o que está tocando.");
            E("Opened Task Manager.", "Abri o Gerenciador de Tarefas.");
            E("That isn't a website.", "Isso não é um site.");
            E("Capri's cursor is on. Capri puts yours back when she starts.", "O cursor da Capri está ligado. Ela devolve o seu quando terminar.");
            P(@"Voice chat failed: (.+)", "O chat de voz falhou: {1}");
            P(@"Voice chat on\. (.+) mutes your mic\.", "Chat de voz ligado. {1} muta seu mic.");
            P(@"Free voice glitched: (.+)", "A voz grátis deu glitch: {1}");
            P(@"Voice glitched: (.+)", "A voz deu glitch: {1}");
            P(@"Still thinking\. Waiting for (.+)\.", "Ainda pensando. Esperando {1}.");
            P(@"Waiting for (.+)\. You can pick another model\.", "Esperando {1}. Você pode escolher outro modelo.");
            P(@"Couldn't open my notes\. They live in (.+)", "Não consegui abrir minhas notas. Elas ficam em {1}");
            P(@"Couldn't read your ElevenLabs voices: (.+)", "Não consegui ler suas vozes da ElevenLabs: {1}");
            P(@"My ears didn't download, so type to me for now\. (.*)", "Meus ouvidos não baixaram, então digita pra mim. {1}");
            P(@"Her own ears didn't download: (.+)", "Os ouvidos dela não baixaram: {1}");
            P(@"Windows speech recognition isn't on this PC: (.+)", "O reconhecimento de fala do Windows não está neste PC: {1}");
            P(@"Free voice chat on, but Windows speech only hears your default mic, not (.+)\. The fix is under VOICE\.", "Chat de voz grátis ligado, mas o Windows só ouve o mic padrão, não {1}. A solução está em VOZ.");
            P(@"Get her free voice \((.+)\), or pick ELEVENLABS\.", "Baixe a voz grátis ({1}) ou escolha ELEVENLABS.");
            P(@"ElevenLabs · (.+)", "ElevenLabs · {1}");
            P(@"Shutting your PC down in (.+?)\.(.*)", "Desligando seu PC em {1}.{2}");
            P(@"Restarting your PC in (.+?)\.(.*)", "Reiniciando seu PC em {1}.{2}");
            P(@"Signing you out in (.+?)\.(.*)", "Saindo da sua conta em {1}.{2}");
            P(@"Putting your PC to sleep in (.+?)\.(.*)", "Colocando seu PC pra dormir em {1}.{2}");
            P(@"Locking your PC in (.+?)\.(.*)", "Bloqueando seu PC em {1}.{2}");
            E(" Say cancel to stop it.", " Diga cancelar para parar.");
            E(" Say cancel the shutdown to stop it.", " Diga cancelar o desligamento para parar.");
            E("10 seconds", "10 segundos"); E("5 seconds", "5 segundos");
            P(@"(\d+) minutes", "{1} minutos"); P(@"(\d+) minute", "{1} minuto"); P(@"(\d+) hours", "{1} horas"); P(@"(\d+) hour", "{1} hora");
            P(@"Playing (.+) on Spotify\.", "Tocando {1} no Spotify.");
            P(@"Spotify is playing (.+) by (.+)\.?", "O Spotify está tocando {1}, de {2}.");
            P(@"Spotify isn't installed, so playing (.+) on YouTube\.", "O Spotify não está instalado, então vou tocar {1} no YouTube.");
            P(@"Couldn't find (.+) to play, so Spotify is open on a search for it\.", "Não achei {1}, então abri uma busca no Spotify.");
            P(@"Couldn't find (.+) to play, so YouTube is open on a search for it\.", "Não achei {1}, então abri uma busca no YouTube.");
            P(@"No app called (.+) is installed on this PC\.", "Nenhum app chamado {1} está instalado neste PC.");
            P(@"More than one app matches (.+)\. Which one\?", "Mais de um app combina com {1}. Qual deles?");
            P(@"(.+) isn't open\.", "{1} não está aberto.");
            P(@"Switched to (.+)\.?", "Mudei para {1}.");
            P(@"Minimized (.+)\.?", "Minimizei {1}.");
            P(@"Closed the (.+) tab\.", "Fechei a aba {1}.");
            P(@"Closed (.+)\.?", "Fechei {1}.");
            P(@"Opened your own website, (.+)", "Abri meu próprio site, {1}");
            P(@"Searched (.+) for (.+)\.?", "Busquei {2} no {1}.");
            P(@"Got stuck on that one\. (.*)", "Travei nessa. {1}");
            P(@"Got lost before I could start that job\. (.*)", "Me perdi antes de começar a tarefa. {1}");
            P(@"Got lost partway through that\. (.*)", "Me perdi no meio da tarefa. {1}");
            P(@"Couldn't get it done\. (.*)", "Não consegui terminar. {1}");
            P(@"Ran out of steps\. Last one: (.+)", "Acabaram os passos. O último: {1}");
            P(@"Done\. (.+)", "Pronto. {1}");

            // ---------------------------------------------------------------- painel de tarefas e controle do PC
            E("working on your task.", "trabalhando na sua tarefa.");
            E("done.", "pronto."); E("stopped.", "parei.");
            E("ESC STOPS ME", "ESC ME PARA");
            E("Looking at the screen", "Olhando a tela");
            E("Thinking", "Pensando");
            E("Looking closer", "Olhando de perto");
            E("Waiting for it to load.", "Esperando carregar.");
            E("Working.", "Trabalhando.");
            E("Clicking.", "Clicando.");
            E("Clicking", "Clicando"); E("Double-clicking", "Clique duplo"); E("Triple-clicking", "Clique triplo"); E("Right-clicking", "Clique direito");
            E("Middle-clicking", "Clique do meio"); E("Dragging", "Arrastando"); E("Waiting", "Esperando");
            E("Clicked", "Cliquei"); E("Double-clicked", "Clique duplo feito"); E("Right-clicked", "Clique direito feito"); E("Dragged", "Arrastei"); E("Waited", "Esperei");
            E("Waited for it to load", "Esperei carregar");
            P(@"CAPRI IS CONTROLLING (.+)", "CAPRI ESTÁ CONTROLANDO {1}");
            P(@"(\d+) earlier steps", "{1} passos anteriores");
            P(@"Clicking (.+)", "Clicando {1}");
            P(@"Typing (.+)", "Digitando {1}");
            P(@"Pressing (.+)", "Apertando {1}");
            P(@"Scrolling (.+)", "Rolando {1}");
            P(@"Opening (.+)", "Abrindo {1}");
            P(@"Typed (.+) and pressed Enter\.?", "Digitei {1} e apertei Enter");
            P(@"Typed (.+)", "Digitei {1}");
            P(@"Pressed (.+)", "Apertei {1}");
            P(@"Scrolled (.+)", "Rolei {1}");
            P(@"Opened (.+) and searched (.+)", "Abri {1} e busquei {2}");
            P(@"Opened (.+)", "Abri {1}");
            E("the app", "o app"); E("the site", "o site");
            P(@"Should I press (.+)\?", "Posso apertar {1}?");
            P(@"Should I send (.+)\?", "Posso enviar {1}?");
            E("Should I send that?", "Posso enviar isso?");

            // ---------------------------------------------------------------- voz, ouvidos e cérebro local (status no Mainframe)
            E("ElevenLabs rejected the key. Press Forget, paste a new one.", "A ElevenLabs recusou a chave. Clique em Esquecer e cole outra.");
            E("ElevenLabs rejected that key. Copy it again from ElevenLabs and paste it.", "A ElevenLabs recusou essa chave. Copie de novo na ElevenLabs e cole.");
            E("ElevenLabs has restricted your account, so her voice can't connect. Sign in at elevenlabs.io to see why, or pick FREE VOICE in the Mainframe.",
              "A ElevenLabs restringiu sua conta, então a voz dela não conecta. Entre em elevenlabs.io para ver o motivo ou escolha VOZ GRÁTIS no Mainframe.");
            E("ElevenLabs is busy. Try again in a moment.", "A ElevenLabs está ocupada. Tenta de novo daqui a pouco.");
            E("That reply had nothing to say out loud.", "Essa resposta não tinha nada para falar em voz alta.");
            E("ElevenLabs only lets paid plans use Voice Library voices in apps. Pick a voice you made or a default one.", "A ElevenLabs só libera vozes da Voice Library em apps nos planos pagos. Escolha uma voz sua ou uma padrão.");
            E("Still failing? Make a new key with Restrict key turned off.", "Continua falhando? Crie uma chave nova com Restrict key desligado.");
            E("ElevenLabs sent a reply Capri couldn't read. Right-click her, Help, Copy error details, and send it in the Discord.", "A ElevenLabs mandou uma resposta que a Capri não entendeu. Clique nela com o botão direito, Ajuda, Copiar detalhes do erro, e mande no Discord.");
            P(@"Voice link closed: (.+)", "A voz desconectou: {1}");
            P(@"Voice link dropped: (.+)", "A voz caiu: {1}");
            P(@"ElevenLabs error (.+)", "Erro da ElevenLabs {1}");
            E("Her own", "A dela"); E("Bright", "Alegre"); E("Sly", "Sapeca"); E("Sharp", "Afiada");
            E("Unpacking her free voice", "Descompactando a voz grátis");
            E("Unpacking her ears", "Descompactando os ouvidos");
            E("Can't open your speakers: Windows has no speakers or headphones on. Pick one in Settings > System > Sound.", "Não consigo abrir seus alto-falantes: o Windows não tem alto-falante nem fone ligado. Escolha um em Configurações > Sistema > Som.");
            E("Can't open your speakers: another app has them to itself. Close it, or untick exclusive mode in your speaker's Sound settings.", "Não consigo abrir seus alto-falantes: outro app está usando em modo exclusivo. Feche o app ou desmarque o modo exclusivo nas configurações de Som.");
            E("Windows has no offline speech recognizer for your language. Turn on Online speech recognition in Settings > Privacy & security > Speech.", "O Windows não tem reconhecimento de fala offline para seu idioma. Ative o Reconhecimento de fala online em Configurações > Privacidade e segurança > Fala.");
            E("Online speech recognition is off in Windows. Turn it on in Settings > Privacy & security > Speech.", "O Reconhecimento de fala online está desligado no Windows. Ative em Configurações > Privacidade e segurança > Fala.");
            E("Windows is blocking the mic. Turn on Let desktop apps access your microphone in Settings > Privacy & security > Microphone.", "O Windows está bloqueando o mic. Ative Permitir que apps da área de trabalho acessem o microfone em Configurações > Privacidade e segurança > Microfone.");
            E("Windows can't find a mic. Plug one in, or pick one in Settings > System > Sound.", "O Windows não acha um mic. Conecte um ou escolha em Configurações > Sistema > Som.");
            E("Windows speech recognition lost its internet connection.", "O reconhecimento de fala do Windows perdeu a internet.");
            E("Windows speech recognition couldn't use the mic.", "O reconhecimento de fala do Windows não conseguiu usar o mic.");
            P(@"Windows speech recognition didn't start: (.+)", "O reconhecimento de fala do Windows não iniciou: {1}");
            P(@"Windows speech only hears your default mic, not (.+)\. Make (.+) the default in Windows Settings > System > Sound > Input, or pick Mic: default here\.",
              "O Windows só ouve o mic padrão, não {1}. Deixe {2} como padrão em Configurações > Sistema > Som > Entrada, ou escolha Mic: padrão aqui.");
            P(@"Can't open your speakers \(Windows error (.+)\)\. Check Settings > System > Sound, then try again\.", "Não consigo abrir seus alto-falantes (erro do Windows {1}). Confira Configurações > Sistema > Som e tente de novo.");
            E("The graphics card garbled replies, so the brain runs on the CPU. Delete this file to try the graphics card again.", "A placa de vídeo embaralhou as respostas, então o cérebro roda no processador. Apague este arquivo para testar a placa de vídeo de novo.");
            E("her eyes", "os olhos dela"); E("the engine", "o motor"); E("her free voice", "a voz grátis"); E("her ears", "os ouvidos");
            E("Her tools are off.", "As ferramentas dela estão desligadas.");

            // por último: "Algo..." traduz o "Algo"
            P(@"(.+[^.])\.\.\.", "{1}...");
        }
    }

    // Menus da Capri: mesmas cores do original, texto em português na hora de desenhar.
    internal sealed class PtMenuRenderer : ToolStripProfessionalRenderer
    {
        internal PtMenuRenderer(ProfessionalColorTable colours) : base(colours) { }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.Text = Pt.T(e.Text);
            base.OnRenderItemText(e);
        }
    }

    // Voz grátis em português: usa uma voz pt-BR instalada no Windows (como a Microsoft Maria), que roda offline.
    // A voz grátis original (Kokoro) só foi treinada em inglês e chinês, então lendo português soaria estranha.
    internal static class PtVoice
    {
        private static readonly object gate = new object();
        private static string voiceName;
        private static bool looked;

        internal static bool Available { get { return Find() != null; } }

        internal static string Name { get { return Find(); } }

        private static string Find()
        {
            lock (gate)
            {
                if (looked) return voiceName;
                looked = true;
                try
                {
                    using (System.Speech.Synthesis.SpeechSynthesizer synth = new System.Speech.Synthesis.SpeechSynthesizer())
                    {
                        string fallback = null;
                        foreach (System.Speech.Synthesis.InstalledVoice installed in synth.GetInstalledVoices())
                        {
                            if (!installed.Enabled) continue;
                            System.Speech.Synthesis.VoiceInfo info = installed.VoiceInfo;
                            if (info.Culture == null || !info.Culture.Name.StartsWith("pt", StringComparison.OrdinalIgnoreCase)) continue;
                            if (info.Culture.Name.Equals("pt-BR", StringComparison.OrdinalIgnoreCase) && info.Gender == System.Speech.Synthesis.VoiceGender.Female) { voiceName = info.Name; break; }
                            if (fallback == null || info.Culture.Name.Equals("pt-BR", StringComparison.OrdinalIgnoreCase)) fallback = info.Name;
                        }
                        if (voiceName == null) voiceName = fallback;
                    }
                }
                catch (Exception) { voiceName = null; }
                return voiceName;
            }
        }

        // Devolve um WAV 16 bits mono a 24 kHz, o mesmo formato que a voz grátis original entrega.
        internal static byte[] Speak(string text)
        {
            string name = Find();
            if (name == null || string.IsNullOrEmpty(text)) return null;
            lock (gate)
            {
                using (System.Speech.Synthesis.SpeechSynthesizer synth = new System.Speech.Synthesis.SpeechSynthesizer())
                using (MemoryStream pcm = new MemoryStream())
                {
                    synth.SelectVoice(name);
                    synth.Rate = 1;
                    synth.SetOutputToAudioStream(pcm, new System.Speech.AudioFormat.SpeechAudioFormatInfo(24000, System.Speech.AudioFormat.AudioBitsPerSample.Sixteen, System.Speech.AudioFormat.AudioChannel.Mono));
                    synth.Speak(text);
                    synth.SetOutputToNull();
                    byte[] raw = pcm.ToArray();
                    short[] samples = new short[raw.Length / 2];
                    Buffer.BlockCopy(raw, 0, samples, 0, samples.Length * 2);
                    return Voice.Wave(samples, 24000);
                }
            }
        }
    }
}
