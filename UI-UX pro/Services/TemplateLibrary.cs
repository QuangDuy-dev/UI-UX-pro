namespace UI_UX_pro.Services;

public class AnimationTemplate
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string CategorySlug { get; init; }
    public required string CategoryName { get; init; }
    public required string[] Tags { get; init; }
    public required string Html { get; init; }
    public required string Css { get; init; }
    public string Js { get; init; } = "";
}

public static class TemplateLibrary
{
    public static readonly Dictionary<string, string> CategoryNames = new()
    {
        ["nav"] = "Navigation",
        ["hero"] = "Hero Section",
        ["button"] = "Button",
        ["card"] = "Card",
        ["list"] = "List",
        ["table"] = "Table",
        ["form"] = "Form",
        ["loader"] = "Loader",
        ["modal"] = "Modal",
        ["toast"] = "Toast",
        ["tabs"] = "Tabs",
        ["accordion"] = "Accordion",
        ["badge"] = "Badge",
        ["progress"] = "Progress",
        ["carousel"] = "Carousel",
        ["marquee"] = "Marquee",
        ["counter"] = "Counter",
        ["chat"] = "Chat",
        ["footer"] = "Footer",
        ["dropdown"] = "Dropdown",
        ["pricing"] = "Pricing",
        ["scroll"] = "Scroll Effect"
    };

    public static readonly List<AnimationTemplate> All = new()
    {
        new AnimationTemplate
        {
            Name = "Glassmorphism Navbar",
            Description = "Sticky navbar with frosted-glass blur and animated underline on hover.",
            CategorySlug = "nav", CategoryName = "Navigation",
            Tags = new[] { "glassmorphism", "navbar", "sticky", "blur" },
            Html = """
                <nav class="glass-nav">
                  <a class="brand" href="#">✦ LOOM</a>
                  <div class="links">
                    <a href="#">Home</a>
                    <a href="#">Work</a>
                    <a href="#">Studio</a>
                    <a href="#">Contact</a>
                  </div>
                  <button class="cta">Get Started</button>
                </nav>
                <div class="spacer"></div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; background: linear-gradient(135deg, #1a1a2e 0%, #16213e 55%, #0f3460 100%); font-family: 'Segoe UI', system-ui, sans-serif; }
                .glass-nav { position: sticky; top: 0; display: flex; align-items: center; justify-content: space-between; gap: 16px; padding: 14px 28px; margin: 18px; border-radius: 18px; background: rgba(255,255,255,.08); backdrop-filter: blur(18px) saturate(160%); -webkit-backdrop-filter: blur(18px) saturate(160%); border: 1px solid rgba(255,255,255,.14); box-shadow: 0 8px 32px rgba(0,0,0,.35); animation: dropIn .6s cubic-bezier(.22,1,.36,1) both; }
                @keyframes dropIn { from { transform: translateY(-18px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }
                .brand { color: #fff; font-weight: 800; letter-spacing: 2px; text-decoration: none; font-size: 17px; background: linear-gradient(90deg,#7dd3fc,#c084fc); -webkit-background-clip: text; background-clip: text; -webkit-text-fill-color: transparent; }
                .links { display: flex; gap: 6px; }
                .links a { position: relative; color: rgba(255,255,255,.78); text-decoration: none; padding: 8px 14px; border-radius: 10px; font-size: 14px; transition: color .25s, background .25s; }
                .links a::after { content: ''; position: absolute; left: 14px; right: 14px; bottom: 4px; height: 2px; border-radius: 2px; background: linear-gradient(90deg,#7dd3fc,#c084fc); transform: scaleX(0); transform-origin: left; transition: transform .3s cubic-bezier(.22,1,.36,1); }
                .links a:hover { color: #fff; background: rgba(255,255,255,.08); }
                .links a:hover::after { transform: scaleX(1); }
                .cta { border: 0; cursor: pointer; color: #fff; font-weight: 600; font-size: 14px; padding: 10px 20px; border-radius: 12px; background: linear-gradient(135deg,#6366f1,#8b5cf6); box-shadow: 0 4px 18px rgba(99,102,241,.45); transition: transform .2s, box-shadow .2s; }
                .cta:hover { transform: translateY(-2px); box-shadow: 0 8px 26px rgba(99,102,241,.6); }
                .spacer { height: 260px; }
                """,
            Js = ""
        },

        new AnimationTemplate
        {
            Name = "Gradient Hero with Floating Shapes",
            Description = "Hero headline with animated gradient text and floating blob shapes in the background.",
            CategorySlug = "hero", CategoryName = "Hero Section",
            Tags = new[] { "hero", "gradient", "floating", "blob" },
            Html = """
                <section class="hero">
                  <div class="blob b1"></div><div class="blob b2"></div><div class="blob b3"></div>
                  <p class="eyebrow">✦ UI/UX Trends 2026</p>
                  <h1>Design that <span class="grad">moves</span> people.</h1>
                  <p class="sub">Beautiful micro-interactions, ready to copy. No frameworks needed.</p>
                  <div class="actions">
                    <button class="btn primary">Browse Gallery</button>
                    <button class="btn ghost">Watch Showreel ▸</button>
                  </div>
                </section>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0b0f1a; overflow: hidden; }
                .hero { position: relative; min-height: 420px; display: flex; flex-direction: column; align-items: center; justify-content: center; text-align: center; padding: 60px 24px; color: #fff; }
                .blob { position: absolute; border-radius: 50%; filter: blur(70px); opacity: .55; animation: float 9s ease-in-out infinite; }
                .b1 { width: 300px; height: 300px; background: #6366f1; top: -60px; left: -60px; }
                .b2 { width: 260px; height: 260px; background: #ec4899; bottom: -40px; right: -40px; animation-delay: -3s; }
                .b3 { width: 180px; height: 180px; background: #22d3ee; top: 40%; left: 62%; animation-delay: -6s; }
                @keyframes float { 0%,100% { transform: translate(0,0) scale(1); } 33% { transform: translate(24px,-30px) scale(1.08); } 66% { transform: translate(-20px,18px) scale(.95); } }
                .eyebrow { position: relative; z-index: 1; color: #22d3ee; font-size: 12px; letter-spacing: 3px; text-transform: uppercase; font-weight: 700; animation: fadeUp .6s .1s both; }
                h1 { position: relative; z-index: 1; font-size: clamp(32px, 6vw, 58px); margin: 14px 0 10px; line-height: 1.1; animation: fadeUp .7s .2s both; }
                .grad { background: linear-gradient(90deg,#22d3ee,#6366f1,#ec4899,#22d3ee); background-size: 300% 100%; -webkit-background-clip: text; background-clip: text; -webkit-text-fill-color: transparent; animation: shift 5s linear infinite; }
                @keyframes shift { to { background-position: 300% 0; } }
                .sub { position: relative; z-index: 1; color: rgba(255,255,255,.65); max-width: 480px; margin: 0 0 26px; animation: fadeUp .7s .3s both; }
                .actions { position: relative; z-index: 1; display: flex; gap: 14px; animation: fadeUp .7s .4s both; }
                .btn { border: 0; cursor: pointer; font-size: 14px; font-weight: 600; padding: 13px 26px; border-radius: 14px; transition: transform .2s, box-shadow .2s; }
                .primary { color: #fff; background: linear-gradient(135deg,#6366f1,#8b5cf6); box-shadow: 0 6px 22px rgba(99,102,241,.5); }
                .primary:hover { transform: translateY(-3px) scale(1.02); box-shadow: 0 12px 30px rgba(99,102,241,.65); }
                .ghost { color: #fff; background: rgba(255,255,255,.08); border: 1px solid rgba(255,255,255,.18); }
                .ghost:hover { transform: translateY(-3px); background: rgba(255,255,255,.14); }
                @keyframes fadeUp { from { transform: translateY(16px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }
                """,
            Js = ""
        },

        new AnimationTemplate
        {
            Name = "Typewriter Hero",
            Description = "Hero with a looping typewriter effect for rotating keywords.",
            CategorySlug = "hero", CategoryName = "Hero Section",
            Tags = new[] { "typewriter", "hero", "typing" },
            Html = """
                <div class="tw">
                  <h1>We build <span class="type" id="type"></span><span class="caret">|</span></h1>
                  <p>Animated text components for modern interfaces.</p>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'JetBrains Mono', 'Consolas', monospace; background: #0d1117; display: flex; align-items: center; justify-content: center; min-height: 100vh; color: #e6edf3; }
                .tw { text-align: center; padding: 24px; }
                h1 { font-size: clamp(22px, 4vw, 38px); font-weight: 600; }
                .type { color: #58a6ff; }
                .caret { color: #58a6ff; animation: blink .8s steps(1) infinite; }
                @keyframes blink { 50% { opacity: 0; } }
                p { color: #8b949e; }
                """,
            Js = """
                const words = ['animations.', 'micro-interactions.', 'delight.', 'interfaces.'];
                const el = document.getElementById('type');
                let wi = 0, ci = 0, deleting = false;
                (function tick() {
                  const word = words[wi];
                  el.textContent = word.slice(0, ci);
                  if (!deleting && ci === word.length) { deleting = true; setTimeout(tick, 1400); return; }
                  if (deleting && ci === 0) { deleting = false; wi = (wi + 1) % words.length; }
                  ci += deleting ? -1 : 1;
                  setTimeout(tick, deleting ? 45 : 110);
                })();
                """
        },

        new AnimationTemplate
        {
            Name = "Magnetic Button",
            Description = "Button that magnetically follows the cursor and springs back on leave.",
            CategorySlug = "button", CategoryName = "Button",
            Tags = new[] { "magnetic", "button", "hover", "cursor" },
            Html = """<button class="mag" id="mag">✨ Hover Me</button>""",
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0f1222; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .mag { position: relative; border: 0; cursor: pointer; color: #fff; font-size: 16px; font-weight: 700; padding: 16px 38px; border-radius: 999px; background: radial-gradient(120% 160% at 20% 0%, #8b5cf6, #4f46e5 60%); box-shadow: 0 10px 30px rgba(99,102,241,.45); transition: transform .35s cubic-bezier(.22,1,.36,1), box-shadow .35s; will-change: transform; }
                .mag:hover { box-shadow: 0 16px 44px rgba(99,102,241,.65); }
                .mag::after { content: ''; position: absolute; inset: 0; border-radius: inherit; background: linear-gradient(120deg, transparent 30%, rgba(255,255,255,.35) 50%, transparent 70%); transform: translateX(-120%); transition: transform .6s; }
                .mag:hover::after { transform: translateX(120%); }
                """,
            Js = """
                const btn = document.getElementById('mag');
                btn.addEventListener('mousemove', e => {
                  const r = btn.getBoundingClientRect();
                  const x = e.clientX - r.left - r.width / 2;
                  const y = e.clientY - r.top - r.height / 2;
                  btn.style.transform = `translate(${x * .3}px, ${y * .4}px)`;
                });
                btn.addEventListener('mouseleave', () => { btn.style.transform = 'translate(0,0)'; });
                """
        },

        new AnimationTemplate
        {
            Name = "Shine Sweep Button",
            Description = "Button with a diagonal shine sweep on hover and a subtle press effect.",
            CategorySlug = "button", CategoryName = "Button",
            Tags = new[] { "shine", "button", "sweep", "hover" },
            Html = """<button class="shine">Download Free</button>""",
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #101418; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .shine { position: relative; overflow: hidden; border: 0; cursor: pointer; color: #101418; font-weight: 800; font-size: 15px; padding: 15px 34px; border-radius: 12px; background: #facc15; box-shadow: 0 8px 24px rgba(250,204,21,.35); transition: transform .15s, box-shadow .25s; }
                .shine::before { content: ''; position: absolute; top: 0; left: -80%; width: 55%; height: 100%; background: linear-gradient(115deg, transparent, rgba(255,255,255,.85), transparent); transform: skewX(-20deg); transition: left .55s ease; }
                .shine:hover::before { left: 130%; }
                .shine:hover { box-shadow: 0 12px 34px rgba(250,204,21,.5); }
                .shine:active { transform: scale(.96); }
                """,
            Js = ""
        },

        new AnimationTemplate
        {
            Name = "Neumorphism Button",
            Description = "Soft neumorphic button that depresses into the surface when pressed.",
            CategorySlug = "button", CategoryName = "Button",
            Tags = new[] { "neumorphism", "button", "soft-ui" },
            Html = """<button class="neo">💜 Press Me</button>""",
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #e8edf3; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .neo { border: 0; cursor: pointer; font-size: 15px; font-weight: 700; color: #5b6b81; padding: 18px 40px; border-radius: 18px; background: #e8edf3; box-shadow: 9px 9px 20px #c3cad6, -9px -9px 20px #ffffff; transition: box-shadow .18s, transform .18s; }
                .neo:hover { box-shadow: 6px 6px 14px #c3cad6, -6px -6px 14px #ffffff; }
                .neo:active { transform: scale(.98); box-shadow: inset 6px 6px 12px #c3cad6, inset -6px -6px 12px #ffffff; }
                """,
            Js = ""
        },

        new AnimationTemplate
        {
            Name = "3D Tilt Card",
            Description = "Card with a 3D tilt that follows the cursor and a glare highlight.",
            CategorySlug = "card", CategoryName = "Card",
            Tags = new[] { "3d", "tilt", "card", "glare" },
            Html = """
                <div class="stage">
                  <div class="tilt" id="tilt">
                    <div class="glare" id="glare"></div>
                    <span class="emoji">🚀</span>
                    <h3>Tilt Card</h3>
                    <p>Hover to feel the depth. Pure CSS + a splash of JS.</p>
                  </div>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #151a26; display: flex; align-items: center; justify-content: center; min-height: 100vh; perspective: 900px; }
                .stage { perspective: 900px; }
                .tilt { position: relative; width: 260px; padding: 34px 26px; border-radius: 22px; background: linear-gradient(160deg, #232b40, #1a2033); border: 1px solid rgba(255,255,255,.09); color: #fff; transform-style: preserve-3d; transition: transform .12s ease-out, box-shadow .3s; box-shadow: 0 24px 60px rgba(0,0,0,.5); }
                .tilt:hover { box-shadow: 0 34px 80px rgba(99,102,241,.35); }
                .emoji { font-size: 40px; display: block; margin-bottom: 12px; transform: translateZ(40px); }
                h3 { margin: 0 0 8px; transform: translateZ(30px); }
                p { margin: 0; color: rgba(255,255,255,.6); font-size: 13px; line-height: 1.6; transform: translateZ(22px); }
                .glare { position: absolute; inset: 0; border-radius: inherit; background: radial-gradient(circle at var(--gx,50%) var(--gy,50%), rgba(255,255,255,.18), transparent 55%); opacity: 0; transition: opacity .3s; pointer-events: none; }
                .tilt:hover .glare { opacity: 1; }
                """,
            Js = """
                const card = document.getElementById('tilt');
                const glare = document.getElementById('glare');
                card.addEventListener('mousemove', e => {
                  const r = card.getBoundingClientRect();
                  const px = (e.clientX - r.left) / r.width;
                  const py = (e.clientY - r.top) / r.height;
                  const rx = (0.5 - py) * 16;
                  const ry = (px - 0.5) * 16;
                  card.style.transform = `rotateX(${rx}deg) rotateY(${ry}deg)`;
                  glare.style.setProperty('--gx', `${px * 100}%`);
                  glare.style.setProperty('--gy', `${py * 100}%`);
                });
                card.addEventListener('mouseleave', () => { card.style.transform = 'rotateX(0) rotateY(0)'; });
                """
        },

        new AnimationTemplate
        {
            Name = "Flip Card",
            Description = "Card that flips 3D on hover to reveal content on the back.",
            CategorySlug = "card", CategoryName = "Card",
            Tags = new[] { "flip", "card", "3d" },
            Html = """
                <div class="flip">
                  <div class="inner">
                    <div class="face front">
                      <span>🎴</span>
                      <h3>Hover to flip</h3>
                    </div>
                    <div class="face back">
                      <h3>You found me!</h3>
                      <p>Back-side content with its own style.</p>
                    </div>
                  </div>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #12141c; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .flip { width: 230px; height: 300px; perspective: 1000px; }
                .inner { position: relative; width: 100%; height: 100%; transform-style: preserve-3d; transition: transform .7s cubic-bezier(.4,.2,.2,1); }
                .flip:hover .inner { transform: rotateY(180deg); }
                .face { position: absolute; inset: 0; backface-visibility: hidden; -webkit-backface-visibility: hidden; border-radius: 20px; display: flex; flex-direction: column; align-items: center; justify-content: center; text-align: center; padding: 22px; color: #fff; }
                .front { background: linear-gradient(160deg, #6366f1, #a855f7); }
                .front span { font-size: 46px; }
                .back { background: linear-gradient(160deg, #0ea5e9, #6366f1); transform: rotateY(180deg); }
                .back h3 { margin: 0 0 8px; }
                .back p { font-size: 13px; color: rgba(255,255,255,.85); margin: 0; }
                """,
            Js = ""
        },

        new AnimationTemplate
        {
            Name = "Hover Lift Card",
            Description = "Minimal card that lifts with a soft shadow and image zoom on hover.",
            CategorySlug = "card", CategoryName = "Card",
            Tags = new[] { "card", "lift", "hover", "shadow" },
            Html = """
                <div class="lift-card">
                  <div class="thumb"><span>🏔</span></div>
                  <div class="meta">
                    <h4>Alpine Sunrise</h4>
                    <p>Minimal hover-lift card with layered shadows.</p>
                    <a href="#">Read more →</a>
                  </div>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #f3f5fa; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .lift-card { width: 250px; background: #fff; border-radius: 18px; overflow: hidden; box-shadow: 0 2px 10px rgba(20,30,60,.08); transition: transform .35s cubic-bezier(.22,1,.36,1), box-shadow .35s; }
                .lift-card:hover { transform: translateY(-10px); box-shadow: 0 30px 50px rgba(20,30,60,.18); }
                .thumb { height: 130px; background: linear-gradient(135deg,#38bdf8,#818cf8); display: flex; align-items: center; justify-content: center; font-size: 48px; transition: transform .5s; }
                .lift-card:hover .thumb { transform: scale(1.06); }
                .meta { padding: 18px 20px 22px; }
                .meta h4 { margin: 0 0 6px; color: #1e293b; }
                .meta p { margin: 0 0 12px; font-size: 13px; color: #64748b; line-height: 1.55; }
                .meta a { color: #6366f1; font-size: 13px; font-weight: 600; text-decoration: none; }
                .meta a:hover { text-decoration: underline; }
                """,
            Js = ""
        },

        new AnimationTemplate
        {
            Name = "Stagger List",
            Description = "List items that cascade in with a stagger and slide on hover.",
            CategorySlug = "list", CategoryName = "List",
            Tags = new[] { "list", "stagger", "fade-in" },
            Html = """
                <ul class="stag-list">
                  <li><span class="dot"></span>Design tokens & spacing</li>
                  <li><span class="dot"></span>Micro-interaction library</li>
                  <li><span class="dot"></span>Copy-paste components</li>
                  <li><span class="dot"></span>Daily trend updates</li>
                  <li><span class="dot"></span>Zero dependencies</li>
                </ul>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0f172a; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .stag-list { list-style: none; margin: 0; padding: 0; width: 300px; }
                .stag-list li { display: flex; align-items: center; gap: 12px; background: rgba(255,255,255,.05); border: 1px solid rgba(255,255,255,.08); color: #e2e8f0; padding: 14px 16px; border-radius: 12px; margin-bottom: 10px; font-size: 14px; opacity: 0; transform: translateX(-16px); animation: slideIn .5s cubic-bezier(.22,1,.36,1) forwards; transition: background .2s, transform .2s; }
                .stag-list li:nth-child(1) { animation-delay: .05s; } .stag-list li:nth-child(2) { animation-delay: .15s; }
                .stag-list li:nth-child(3) { animation-delay: .25s; } .stag-list li:nth-child(4) { animation-delay: .35s; }
                .stag-list li:nth-child(5) { animation-delay: .45s; }
                @keyframes slideIn { to { opacity: 1; transform: translateX(0); } }
                .stag-list li:hover { background: rgba(99,102,241,.18); transform: translateX(6px); }
                .dot { width: 10px; height: 10px; border-radius: 50%; background: linear-gradient(135deg,#22d3ee,#6366f1); box-shadow: 0 0 12px rgba(99,102,241,.8); }
                """,
            Js = ""
        },

        new AnimationTemplate
        {
            Name = "Animated Timeline",
            Description = "Vertical timeline with pulsing dots and growing progress line.",
            CategorySlug = "list", CategoryName = "List",
            Tags = new[] { "timeline", "vertical", "dots" },
            Html = """
                <div class="timeline">
                  <div class="tl-item"><span class="tl-dot"></span><div class="tl-body"><h4>Discovery</h4><p>Research & user interviews.</p></div></div>
                  <div class="tl-item"><span class="tl-dot"></span><div class="tl-body"><h4>Design</h4><p>Wireframes, prototypes, motion.</p></div></div>
                  <div class="tl-item"><span class="tl-dot"></span><div class="tl-body"><h4>Launch</h4><p>Ship, measure, iterate.</p></div></div>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #101623; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .timeline { position: relative; width: 300px; padding-left: 26px; }
                .timeline::before { content: ''; position: absolute; left: 5px; top: 8px; bottom: 8px; width: 2px; background: linear-gradient(#6366f1, #22d3ee); transform-origin: top; animation: grow 1.6s cubic-bezier(.22,1,.36,1) both; }
                @keyframes grow { from { transform: scaleY(0); } to { transform: scaleY(1); } }
                .tl-item { position: relative; margin-bottom: 26px; opacity: 0; animation: fadeIn .6s forwards; }
                .tl-item:nth-child(1) { animation-delay: .4s; } .tl-item:nth-child(2) { animation-delay: .8s; } .tl-item:nth-child(3) { animation-delay: 1.2s; }
                @keyframes fadeIn { to { opacity: 1; } }
                .tl-dot { position: absolute; left: -26px; top: 6px; width: 12px; height: 12px; border-radius: 50%; background: #6366f1; box-shadow: 0 0 0 4px rgba(99,102,241,.25); animation: pulse 2s infinite; }
                .tl-item:nth-child(2) .tl-dot { background: #8b5cf6; box-shadow: 0 0 0 4px rgba(139,92,246,.25); }
                .tl-item:nth-child(3) .tl-dot { background: #22d3ee; box-shadow: 0 0 0 4px rgba(34,211,238,.25); }
                @keyframes pulse { 0% { box-shadow: 0 0 0 0 rgba(99,102,241,.5); } 70% { box-shadow: 0 0 0 10px rgba(99,102,241,0); } 100% { box-shadow: 0 0 0 0 rgba(99,102,241,0); } }
                .tl-body h4 { margin: 0 0 4px; color: #f1f5f9; font-size: 15px; }
                .tl-body p { margin: 0; color: #94a3b8; font-size: 13px; }
                """,
            Js = ""
        },

        new AnimationTemplate
        {
            Name = "Interactive Data Table",
            Description = "Responsive table with row highlight, hover actions and a striped rhythm.",
            CategorySlug = "table", CategoryName = "Table",
            Tags = new[] { "table", "data", "hover", "zebra" },
            Html = """
                <div class="tbl-wrap">
                  <table>
                    <thead><tr><th>Project</th><th>Status</th><th>Progress</th><th>Owner</th></tr></thead>
                    <tbody>
                      <tr><td>Nebula Dashboard</td><td><span class="badge ok">Live</span></td><td>92%</td><td>Mai</td></tr>
                      <tr><td>Orbit Mobile</td><td><span class="badge warn">Beta</span></td><td>64%</td><td>Duy</td></tr>
                      <tr><td>Pulse Design Sys</td><td><span class="badge ok">Live</span></td><td>100%</td><td>Lan</td></tr>
                      <tr><td>Prism Analytics</td><td><span class="badge idle">Draft</span></td><td>18%</td><td>Khoa</td></tr>
                    </tbody>
                  </table>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0d1117; display: flex; align-items: center; justify-content: center; min-height: 100vh; padding: 20px; }
                .tbl-wrap { width: 100%; max-width: 560px; border-radius: 16px; overflow: hidden; border: 1px solid rgba(255,255,255,.08); background: #161b22; }
                table { width: 100%; border-collapse: collapse; font-size: 13px; color: #e6edf3; }
                thead th { text-align: left; padding: 13px 16px; background: #1c2333; color: #8b949e; font-size: 11px; text-transform: uppercase; letter-spacing: 1px; }
                tbody td { padding: 13px 16px; border-top: 1px solid rgba(255,255,255,.05); transition: background .2s, transform .2s; }
                tbody tr:nth-child(even) { background: rgba(255,255,255,.02); }
                tbody tr { transition: background .2s; }
                tbody tr:hover { background: rgba(88,166,255,.1); }
                tbody tr:hover td:first-child { color: #58a6ff; }
                .badge { padding: 3px 10px; border-radius: 999px; font-size: 11px; font-weight: 700; }
                .ok { background: rgba(63,185,80,.15); color: #3fb950; }
                .warn { background: rgba(210,153,34,.15); color: #d29922; }
                .idle { background: rgba(139,148,158,.15); color: #8b949e; }
                """,
            Js = ""
        },

        new AnimationTemplate
        {
            Name = "Sliding Tabs",
            Description = "Tabs with an animated sliding underline indicator that follows clicks.",
            CategorySlug = "tabs", CategoryName = "Tabs",
            Tags = new[] { "tabs", "underline", "slider" },
            Html = """
                <div class="tabs">
                  <div class="tab-head" id="tabHead">
                    <button class="tab active" data-i="0">Overview</button>
                    <button class="tab" data-i="1">Activity</button>
                    <button class="tab" data-i="2">Settings</button>
                    <span class="indicator" id="ind"></span>
                  </div>
                  <div class="tab-panel active" data-p="0"><p>Overview content — quick stats and charts live here.</p></div>
                  <div class="tab-panel" data-p="1"><p>Activity feed — recent changes and events.</p></div>
                  <div class="tab-panel" data-p="2"><p>Settings — preferences and toggles.</p></div>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0f1420; display: flex; align-items: center; justify-content: center; min-height: 100vh; padding: 20px; }
                .tabs { width: 100%; max-width: 460px; background: #171e2e; border: 1px solid rgba(255,255,255,.07); border-radius: 16px; padding: 18px; }
                .tab-head { position: relative; display: flex; gap: 4px; border-bottom: 1px solid rgba(255,255,255,.08); }
                .tab { position: relative; z-index: 1; border: 0; background: none; color: #94a3b8; font-size: 14px; font-weight: 600; padding: 10px 14px; cursor: pointer; transition: color .25s; }
                .tab:hover { color: #e2e8f0; }
                .tab.active { color: #fff; }
                .indicator { position: absolute; bottom: -1px; left: 0; height: 2px; width: 0; border-radius: 2px; background: linear-gradient(90deg,#22d3ee,#6366f1); transition: left .35s cubic-bezier(.22,1,.36,1), width .35s cubic-bezier(.22,1,.36,1); }
                .tab-panel { display: none; animation: fadeSlide .35s ease; }
                .tab-panel.active { display: block; }
                .tab-panel p { color: #94a3b8; font-size: 13px; line-height: 1.6; }
                @keyframes fadeSlide { from { opacity: 0; transform: translateY(6px); } to { opacity: 1; transform: translateY(0); } }
                """,
            Js = """
                const head = document.getElementById('tabHead');
                const ind = document.getElementById('ind');
                const tabs = [...document.querySelectorAll('.tab')];
                const move = () => {
                  const t = document.querySelector('.tab.active');
                  ind.style.left = t.offsetLeft + 'px';
                  ind.style.width = t.offsetWidth + 'px';
                };
                move(); window.addEventListener('resize', move);
                head.addEventListener('click', e => {
                  const b = e.target.closest('.tab'); if (!b) return;
                  tabs.forEach(x => x.classList.remove('active'));
                  b.classList.add('active');
                  document.querySelectorAll('.tab-panel').forEach(p => p.classList.remove('active'));
                  document.querySelector(`.tab-panel[data-p="${b.dataset.i}"]`).classList.add('active');
                  move();
                });
                """
        },

        new AnimationTemplate
        {
            Name = "Smooth Accordion",
            Description = "Accordion with smooth height animation and rotating chevron.",
            CategorySlug = "accordion", CategoryName = "Accordion",
            Tags = new[] { "accordion", "faq", "expand" },
            Html = """
                <div class="acc">
                  <div class="acc-item open">
                    <button class="acc-head">What is this library? <span class="chev">⌄</span></button>
                    <div class="acc-body"><p>A growing collection of animated HTML/CSS/JS components you can copy freely.</p></div>
                  </div>
                  <div class="acc-item">
                    <button class="acc-head">Do I need frameworks? <span class="chev">⌄</span></button>
                    <div class="acc-body"><p>No. Everything is vanilla — paste it into any project.</p></div>
                  </div>
                  <div class="acc-item">
                    <button class="acc-head">How often is it updated? <span class="chev">⌄</span></button>
                    <div class="acc-body"><p>New components are added every day based on current UI/UX trends.</p></div>
                  </div>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #11141c; display: flex; align-items: center; justify-content: center; min-height: 100vh; padding: 20px; }
                .acc { width: 100%; max-width: 440px; }
                .acc-item { margin-bottom: 10px; border: 1px solid rgba(255,255,255,.08); border-radius: 14px; background: rgba(255,255,255,.04); overflow: hidden; transition: border-color .3s, background .3s; }
                .acc-item.open { border-color: rgba(99,102,241,.5); background: rgba(99,102,241,.08); }
                .acc-head { width: 100%; display: flex; justify-content: space-between; align-items: center; gap: 12px; border: 0; background: none; color: #e2e8f0; font-size: 14px; font-weight: 600; padding: 16px; cursor: pointer; text-align: left; }
                .chev { transition: transform .35s cubic-bezier(.22,1,.36,1); color: #818cf8; }
                .acc-item.open .chev { transform: rotate(180deg); }
                .acc-body { display: grid; grid-template-rows: 0fr; transition: grid-template-rows .4s cubic-bezier(.22,1,.36,1); }
                .acc-item.open .acc-body { grid-template-rows: 1fr; }
                .acc-body > div { overflow: hidden; }
                .acc-body p { margin: 0; padding: 0 16px 16px; color: #94a3b8; font-size: 13px; line-height: 1.6; }
                """,
            Js = """
                document.querySelectorAll('.acc-head').forEach(h => {
                  h.addEventListener('click', () => {
                    const item = h.parentElement;
                    const wasOpen = item.classList.contains('open');
                    document.querySelectorAll('.acc-item').forEach(i => i.classList.remove('open'));
                    if (!wasOpen) item.classList.add('open');
                  });
                });
                """
        },

        new AnimationTemplate
        {
            Name = "Skeleton Loader",
            Description = "Card skeleton with shimmering placeholder blocks while content loads.",
            CategorySlug = "loader", CategoryName = "Loader",
            Tags = new[] { "skeleton", "loader", "shimmer", "placeholder" },
            Html = """
                <div class="sk">
                  <div class="sk-img"></div>
                  <div class="sk-line w80"></div>
                  <div class="sk-line w100"></div>
                  <div class="sk-line w60"></div>
                  <div class="sk-row"><div class="sk-avatar"></div><div class="sk-line w40"></div></div>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0f172a; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .sk { width: 260px; padding: 18px; border-radius: 18px; background: #1e293b; display: flex; flex-direction: column; gap: 12px; }
                .sk-img { height: 110px; border-radius: 12px; }
                .sk-line { height: 12px; border-radius: 6px; }
                .sk-row { display: flex; align-items: center; gap: 10px; }
                .sk-avatar { width: 34px; height: 34px; border-radius: 50%; }
                .w80 { width: 80%; } .w100 { width: 100%; } .w60 { width: 60%; } .w40 { width: 40%; }
                .sk-img, .sk-line, .sk-avatar { position: relative; overflow: hidden; background: #283548; }
                .sk-img::after, .sk-line::after, .sk-avatar::after { content: ''; position: absolute; inset: 0; transform: translateX(-100%); background: linear-gradient(90deg, transparent, rgba(255,255,255,.08), transparent); animation: shimmer 1.4s infinite; }
                @keyframes shimmer { 100% { transform: translateX(100%); } }
                """,
            Js = ""
        },

        new AnimationTemplate
        {
            Name = "Gradient Spinner",
            Description = "Conic-gradient ring spinner with a soft glow and rotating core.",
            CategorySlug = "loader", CategoryName = "Loader",
            Tags = new[] { "spinner", "loader", "gradient", "ring" },
            Html = """<div class="spin-wrap"><div class="ring"><div class="core"></div></div><p>Loading…</p></div>""",
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0b0f1a; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .spin-wrap { display: flex; flex-direction: column; align-items: center; gap: 16px; color: #64748b; font-size: 13px; letter-spacing: 1px; }
                .ring { position: relative; width: 72px; height: 72px; border-radius: 50%; background: conic-gradient(from 0deg, transparent 0 70%, #22d3ee 80%, #6366f1 90%, #ec4899 100%); -webkit-mask: radial-gradient(farthest-side, transparent calc(100% - 8px), #000 calc(100% - 7px)); mask: radial-gradient(farthest-side, transparent calc(100% - 8px), #000 calc(100% - 7px)); animation: spin 1s linear infinite; filter: drop-shadow(0 0 10px rgba(99,102,241,.6)); }
                .core { position: absolute; inset: 22px; border-radius: 50%; background: #22d3ee; box-shadow: 0 0 18px rgba(34,211,238,.9); animation: corePulse 1.4s ease-in-out infinite; }
                @keyframes spin { to { transform: rotate(360deg); } }
                @keyframes corePulse { 0%,100% { transform: scale(1); } 50% { transform: scale(.72); } }
                """,
            Js = ""
        },

        new AnimationTemplate
        {
            Name = "Animated Progress Bar",
            Description = "Progress bar that animates to a target percentage with a moving stripe.",
            CategorySlug = "progress", CategoryName = "Progress",
            Tags = new[] { "progress", "bar", "percent" },
            Html = """
                <div class="prog">
                  <div class="prog-top"><span>Uploading assets</span><span id="pct">0%</span></div>
                  <div class="track"><div class="fill" id="fill"></div></div>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0f1420; display: flex; align-items: center; justify-content: center; min-height: 100vh; padding: 20px; }
                .prog { width: 100%; max-width: 380px; }
                .prog-top { display: flex; justify-content: space-between; color: #cbd5e1; font-size: 13px; margin-bottom: 8px; }
                .track { height: 10px; border-radius: 999px; background: rgba(255,255,255,.08); overflow: hidden; }
                .fill { height: 100%; width: 0; border-radius: 999px; background: linear-gradient(90deg,#22d3ee,#6366f1); position: relative; overflow: hidden; transition: width .2s linear; }
                .fill::after { content: ''; position: absolute; inset: 0; background: linear-gradient(90deg, transparent, rgba(255,255,255,.35), transparent); animation: stripe 1.1s linear infinite; }
                @keyframes stripe { from { transform: translateX(-100%); } to { transform: translateX(100%); } }
                """,
            Js = """
                const fill = document.getElementById('fill');
                const pct = document.getElementById('pct');
                let v = 0;
                const timer = setInterval(() => {
                  v += Math.random() * 8 + 2;
                  if (v >= 100) { v = 100; clearInterval(timer); }
                  fill.style.width = v + '%';
                  pct.textContent = Math.round(v) + '%';
                }, 180);
                """
        },

        new AnimationTemplate
        {
            Name = "Toast Notification",
            Description = "Slide-in toast with progress countdown and close button.",
            CategorySlug = "toast", CategoryName = "Toast",
            Tags = new[] { "toast", "notification", "slide-in" },
            Html = """
                <div class="toast-wrap">
                  <div class="toast" id="toast">
                    <span class="t-ico">✅</span>
                    <div class="t-txt"><strong>Saved!</strong><span>Your animation was added to the gallery.</span></div>
                    <button class="t-x" id="tClose">✕</button>
                    <div class="t-progress" id="tProg"></div>
                  </div>
                </div>
                <button class="t-trigger" id="tTrigger">Show toast</button>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #10151f; display: flex; flex-direction: column; align-items: center; justify-content: center; min-height: 100vh; gap: 20px; }
                .toast-wrap { position: fixed; top: 18px; right: 18px; }
                .toast { position: relative; display: flex; align-items: center; gap: 12px; width: 320px; padding: 14px 40px 14px 14px; border-radius: 14px; background: #1c2434; border: 1px solid rgba(255,255,255,.09); box-shadow: 0 18px 44px rgba(0,0,0,.45); color: #e2e8f0; transform: translateX(140%); transition: transform .5s cubic-bezier(.22,1,.36,1); }
                .toast.show { transform: translateX(0); }
                .t-ico { font-size: 20px; }
                .t-txt { display: flex; flex-direction: column; font-size: 13px; }
                .t-txt strong { font-size: 14px; }
                .t-txt span { color: #94a3b8; }
                .t-x { position: absolute; top: 8px; right: 10px; border: 0; background: none; color: #64748b; cursor: pointer; font-size: 13px; }
                .t-x:hover { color: #fff; }
                .t-progress { position: absolute; left: 0; bottom: 0; height: 3px; width: 100%; background: linear-gradient(90deg,#22d3ee,#6366f1); transform-origin: left; transform: scaleX(0); }
                .toast.show .t-progress { animation: shrink 4s linear forwards; }
                @keyframes shrink { from { transform: scaleX(1); } to { transform: scaleX(0); } }
                .t-trigger { border: 0; cursor: pointer; color: #fff; font-weight: 600; padding: 12px 24px; border-radius: 12px; background: linear-gradient(135deg,#6366f1,#8b5cf6); box-shadow: 0 6px 20px rgba(99,102,241,.45); }
                """,
            Js = """
                const toast = document.getElementById('toast');
                let timer;
                const show = () => {
                  toast.classList.add('show');
                  clearTimeout(timer);
                  timer = setTimeout(hide, 4000);
                };
                const hide = () => toast.classList.remove('show');
                document.getElementById('tTrigger').addEventListener('click', show);
                document.getElementById('tClose').addEventListener('click', hide);
                """
        },

        new AnimationTemplate
        {
            Name = "Pop Modal",
            Description = "Modal that pops in with scale + blur backdrop and slide-up content.",
            CategorySlug = "modal", CategoryName = "Modal",
            Tags = new[] { "modal", "popup", "dialog" },
            Html = """
                <button class="m-open" id="mOpen">Open Modal</button>
                <div class="m-backdrop" id="mBack">
                  <div class="m-box">
                    <button class="m-close" id="mClose">✕</button>
                    <span class="m-emoji">🎉</span>
                    <h3>Welcome aboard!</h3>
                    <p>This modal scales in smoothly. Click outside or press Esc to close.</p>
                    <button class="m-cta" id="mCta">Got it</button>
                  </div>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: linear-gradient(135deg,#141b2d,#0f1420); display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .m-open { border: 0; cursor: pointer; color: #fff; font-weight: 600; padding: 13px 26px; border-radius: 12px; background: linear-gradient(135deg,#6366f1,#8b5cf6); box-shadow: 0 8px 24px rgba(99,102,241,.5); }
                .m-backdrop { position: fixed; inset: 0; display: flex; align-items: center; justify-content: center; background: rgba(5,8,15,.6); backdrop-filter: blur(6px); opacity: 0; pointer-events: none; transition: opacity .3s; }
                .m-backdrop.show { opacity: 1; pointer-events: auto; }
                .m-box { position: relative; width: 320px; background: #1b2334; border: 1px solid rgba(255,255,255,.1); border-radius: 20px; padding: 34px 28px 28px; text-align: center; color: #e2e8f0; transform: scale(.85) translateY(16px); transition: transform .4s cubic-bezier(.22,1,.36,1); box-shadow: 0 30px 70px rgba(0,0,0,.6); }
                .m-backdrop.show .m-box { transform: scale(1) translateY(0); }
                .m-close { position: absolute; top: 12px; right: 14px; border: 0; background: none; color: #64748b; font-size: 16px; cursor: pointer; }
                .m-close:hover { color: #fff; }
                .m-emoji { font-size: 42px; display: block; margin-bottom: 10px; }
                .m-box h3 { margin: 0 0 8px; }
                .m-box p { margin: 0 0 20px; color: #94a3b8; font-size: 13px; line-height: 1.6; }
                .m-cta { width: 100%; border: 0; cursor: pointer; color: #fff; font-weight: 700; padding: 12px; border-radius: 12px; background: linear-gradient(135deg,#22d3ee,#6366f1); }
                """,
            Js = """
                const back = document.getElementById('mBack');
                const show = () => back.classList.add('show');
                const hide = () => back.classList.remove('show');
                document.getElementById('mOpen').addEventListener('click', show);
                document.getElementById('mClose').addEventListener('click', hide);
                document.getElementById('mCta').addEventListener('click', hide);
                back.addEventListener('click', e => { if (e.target === back) hide(); });
                document.addEventListener('keydown', e => { if (e.key === 'Escape') hide(); });
                """
        },

        new AnimationTemplate
        {
            Name = "Infinite Marquee",
            Description = "Seamless infinite marquee strip for logos or keywords, pauses on hover.",
            CategorySlug = "marquee", CategoryName = "Marquee",
            Tags = new[] { "marquee", "infinite", "logos", "scroll" },
            Html = """
                <div class="mq" id="mq">
                  <div class="mq-track" id="mqTrack">
                    <span>✦ GLASSMORPHISM</span><span>✦ MICRO-INTERACTION</span><span>✦ NEUMORPHISM</span><span>✦ 3D TILT</span><span>✦ SCROLL REVEAL</span><span>✦ DARK MODE</span>
                  </div>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0c1017; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .mq { width: 100%; max-width: 640px; overflow: hidden; border-radius: 14px; border: 1px solid rgba(255,255,255,.08); background: #141a26; padding: 16px 0; -webkit-mask-image: linear-gradient(90deg, transparent, #000 12%, #000 88%, transparent); mask-image: linear-gradient(90deg, transparent, #000 12%, #000 88%, transparent); }
                .mq-track { display: flex; gap: 40px; width: max-content; animation: scroll 16s linear infinite; }
                .mq:hover .mq-track { animation-play-state: paused; }
                .mq-track span { color: #94a3b8; font-size: 14px; font-weight: 700; letter-spacing: 2px; white-space: nowrap; }
                @keyframes scroll { to { transform: translateX(-50%); } }
                """,
            Js = """
                const track = document.getElementById('mqTrack');
                track.innerHTML += track.innerHTML;
                """
        },

        new AnimationTemplate
        {
            Name = "Count-Up Stat",
            Description = "Numbers that count up when scrolled into view.",
            CategorySlug = "counter", CategoryName = "Counter",
            Tags = new[] { "counter", "count-up", "stats" },
            Html = """
                <div class="stats">
                  <div class="stat"><span class="num" data-target="12400" data-prefix="">0</span><label>Components</label></div>
                  <div class="stat"><span class="num" data-target="2300" data-prefix="+">0</span><label>Daily visitors</label></div>
                  <div class="stat"><span class="num" data-target="98" data-suffix="%">0</span><label>Satisfaction</label></div>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0f1420; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .stats { display: flex; gap: 26px; flex-wrap: wrap; justify-content: center; }
                .stat { text-align: center; padding: 22px 30px; border-radius: 16px; background: rgba(255,255,255,.04); border: 1px solid rgba(255,255,255,.08); min-width: 130px; }
                .num { display: block; font-size: 34px; font-weight: 800; color: #fff; background: linear-gradient(90deg,#22d3ee,#6366f1); -webkit-background-clip: text; background-clip: text; -webkit-text-fill-color: transparent; }
                .stat label { display: block; margin-top: 6px; color: #94a3b8; font-size: 12px; letter-spacing: 1px; }
                """,
            Js = """
                const nums = document.querySelectorAll('.num');
                const fmt = n => new Intl.NumberFormat('en-US').format(n);
                const run = el => {
                  const target = +el.dataset.target;
                  const dur = 1400, t0 = performance.now();
                  const step = t => {
                    const p = Math.min((t - t0) / dur, 1);
                    const eased = 1 - Math.pow(1 - p, 3);
                    el.textContent = (el.dataset.prefix || '') + fmt(Math.round(target * eased)) + (el.dataset.suffix || '');
                    if (p < 1) requestAnimationFrame(step);
                  };
                  requestAnimationFrame(step);
                };
                const io = new IntersectionObserver(entries => {
                  entries.forEach(en => { if (en.isIntersecting) { run(en.target); io.unobserve(en.target); } });
                }, { threshold: .5 });
                nums.forEach(n => io.observe(n));
                """
        },

        new AnimationTemplate
        {
            Name = "Scroll Reveal Section",
            Description = "Content that fades and slides in as you scroll, with direction variants.",
            CategorySlug = "scroll", CategoryName = "Scroll Effect",
            Tags = new[] { "scroll", "reveal", "intersection" },
            Html = """
                <div class="sr-wrap">
                  <div class="sr" data-dir="left"><h3>Scroll Reveal</h3><p>Elements animate in when they enter the viewport.</p></div>
                  <div class="sr" data-dir="right"><h3>Direction-aware</h3><p>Slide from left or right based on the data attribute.</p></div>
                  <div class="sr" data-dir="up"><h3>Zero dependencies</h3><p>Vanilla IntersectionObserver under the hood.</p></div>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0e1320; color: #e2e8f0; padding: 40px 20px; }
                .sr-wrap { max-width: 420px; margin: 0 auto; }
                .sr { padding: 22px; margin-bottom: 20px; border-radius: 16px; background: rgba(255,255,255,.05); border: 1px solid rgba(255,255,255,.08); opacity: 0; transform: translateY(30px); transition: opacity .6s ease, transform .6s cubic-bezier(.22,1,.36,1); }
                .sr[data-dir="left"] { transform: translateX(-40px); }
                .sr[data-dir="right"] { transform: translateX(40px); }
                .sr.visible { opacity: 1; transform: translate(0,0); }
                .sr h3 { margin: 0 0 6px; font-size: 16px; }
                .sr p { margin: 0; color: #94a3b8; font-size: 13px; line-height: 1.6; }
                """,
            Js = """
                const io = new IntersectionObserver(entries => {
                  entries.forEach(en => { if (en.isIntersecting) { en.target.classList.add('visible'); io.unobserve(en.target); } });
                }, { threshold: .25 });
                document.querySelectorAll('.sr').forEach(el => io.observe(el));
                """
        },

        new AnimationTemplate
        {
            Name = "Pricing Toggle",
            Description = "Monthly/yearly toggle with animated knob and switching prices.",
            CategorySlug = "pricing", CategoryName = "Pricing",
            Tags = new[] { "pricing", "toggle", "switch", "billing" },
            Html = """
                <div class="pricing">
                  <div class="seg" id="seg">
                    <span class="seg-knob" id="segKnob"></span>
                    <button class="seg-btn active" data-v="monthly">Monthly</button>
                    <button class="seg-btn" data-v="yearly">Yearly <b>-20%</b></button>
                  </div>
                  <div class="price-row">
                    <span class="currency">$</span><span class="amount" id="amount">12</span><span class="per">/mo</span>
                  </div>
                  <p class="note" id="note">Billed monthly · cancel anytime</p>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0f1420; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .pricing { width: 300px; text-align: center; padding: 28px 24px; border-radius: 20px; background: rgba(255,255,255,.04); border: 1px solid rgba(255,255,255,.09); }
                .seg { position: relative; display: flex; background: rgba(255,255,255,.06); border-radius: 999px; padding: 4px; }
                .seg-knob { position: absolute; top: 4px; bottom: 4px; left: 4px; width: calc(50% - 4px); border-radius: 999px; background: linear-gradient(135deg,#6366f1,#8b5cf6); box-shadow: 0 4px 14px rgba(99,102,241,.5); transition: transform .35s cubic-bezier(.22,1,.36,1); }
                .seg-btn { position: relative; z-index: 1; flex: 1; border: 0; background: none; color: #94a3b8; font-size: 13px; font-weight: 600; padding: 9px 8px; cursor: pointer; transition: color .3s; }
                .seg-btn.active { color: #fff; }
                .seg-btn b { color: #22d3ee; font-size: 11px; }
                .price-row { margin: 26px 0 6px; color: #fff; display: flex; align-items: baseline; justify-content: center; gap: 2px; }
                .currency { font-size: 20px; font-weight: 700; color: #818cf8; }
                .amount { font-size: 52px; font-weight: 800; line-height: 1; transition: opacity .2s; }
                .per { color: #94a3b8; font-size: 14px; }
                .note { color: #64748b; font-size: 12px; margin: 0; }
                """,
            Js = """
                const seg = document.getElementById('seg');
                const knob = document.getElementById('segKnob');
                const amount = document.getElementById('amount');
                const note = document.getElementById('note');
                const values = { monthly: 12, yearly: 9 };
                seg.addEventListener('click', e => {
                  const b = e.target.closest('.seg-btn'); if (!b) return;
                  document.querySelectorAll('.seg-btn').forEach(x => x.classList.remove('active'));
                  b.classList.add('active');
                  const yearly = b.dataset.v === 'yearly';
                  knob.style.transform = yearly ? 'translateX(100%)' : 'translateX(0)';
                  amount.style.opacity = 0;
                  setTimeout(() => { amount.textContent = yearly ? values.yearly : values.monthly; amount.style.opacity = 1; }, 180);
                  note.textContent = yearly ? 'Billed yearly · save 25%' : 'Billed monthly · cancel anytime';
                });
                """
        },

        new AnimationTemplate
        {
            Name = "Chat Bubbles",
            Description = "Typing indicator with bouncing dots and animated message bubbles.",
            CategorySlug = "chat", CategoryName = "Chat",
            Tags = new[] { "chat", "bubbles", "typing", "messaging" },
            Html = """
                <div class="chat">
                  <div class="msg them"><p>Hey! Have you seen the new components? 👀</p></div>
                  <div class="msg me"><p>Yes! The tilt cards are 🔥</p></div>
                  <div class="typing"><span></span><span></span><span></span></div>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0f1520; display: flex; align-items: center; justify-content: center; min-height: 100vh; padding: 20px; }
                .chat { width: 100%; max-width: 320px; display: flex; flex-direction: column; gap: 10px; }
                .msg { max-width: 78%; padding: 11px 14px; border-radius: 16px; font-size: 13px; line-height: 1.5; opacity: 0; animation: popIn .4s cubic-bezier(.22,1,.36,1) forwards; }
                .msg p { margin: 0; }
                .msg.them { align-self: flex-start; background: #1c2434; color: #e2e8f0; border-bottom-left-radius: 4px; }
                .msg.me { align-self: flex-end; background: linear-gradient(135deg,#6366f1,#8b5cf6); color: #fff; border-bottom-right-radius: 4px; animation-delay: .3s; }
                @keyframes popIn { from { opacity: 0; transform: translateY(10px) scale(.94); } to { opacity: 1; transform: translateY(0) scale(1); } }
                .typing { align-self: flex-start; display: flex; gap: 5px; background: #1c2434; padding: 13px 16px; border-radius: 16px; border-bottom-left-radius: 4px; animation: popIn .4s .6s both; }
                .typing span { width: 7px; height: 7px; border-radius: 50%; background: #64748b; animation: bounce 1.2s infinite; }
                .typing span:nth-child(2) { animation-delay: .15s; } .typing span:nth-child(3) { animation-delay: .3s; }
                @keyframes bounce { 0%,60%,100% { transform: translateY(0); opacity: .5; } 30% { transform: translateY(-6px); opacity: 1; } }
                """,
            Js = ""
        },

        new AnimationTemplate
        {
            Name = "Pulsing Badge",
            Description = "Badge with a pulsing notification dot and hover scale.",
            CategorySlug = "badge", CategoryName = "Badge",
            Tags = new[] { "badge", "notification", "pulse", "dot" },
            Html = """
                <div class="bdg-row">
                  <span class="bdg">New<span class="dot"></span></span>
                  <span class="bdg subtle">Beta</span>
                  <span class="bdg ghost">3.2k ✦</span>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0f1420; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .bdg-row { display: flex; gap: 12px; flex-wrap: wrap; justify-content: center; }
                .bdg { position: relative; display: inline-flex; align-items: center; gap: 8px; padding: 7px 14px; border-radius: 999px; font-size: 12px; font-weight: 700; letter-spacing: .5px; transition: transform .2s; cursor: default; }
                .bdg:hover { transform: scale(1.08); }
                .bdg { background: linear-gradient(135deg,#22d3ee,#6366f1); color: #fff; box-shadow: 0 4px 16px rgba(34,211,238,.35); }
                .bdg.subtle { background: rgba(255,255,255,.08); color: #e2e8f0; border: 1px solid rgba(255,255,255,.12); }
                .bdg.ghost { background: transparent; color: #818cf8; border: 1px dashed #6366f1; }
                .dot { position: relative; width: 8px; height: 8px; border-radius: 50%; background: #fff; }
                .dot::after { content: ''; position: absolute; inset: 0; border-radius: 50%; background: #fff; animation: ping 1.4s cubic-bezier(0,0,.2,1) infinite; }
                @keyframes ping { 0% { transform: scale(1); opacity: .8; } 80%,100% { transform: scale(3); opacity: 0; } }
                """,
            Js = ""
        },

        new AnimationTemplate
        {
            Name = "Animated Dropdown",
            Description = "Dropdown menu that scales and fades in with smooth chevron rotation.",
            CategorySlug = "dropdown", CategoryName = "Dropdown",
            Tags = new[] { "dropdown", "menu", "select" },
            Html = """
                <div class="dd" id="dd">
                  <button class="dd-trigger" id="ddTrigger">Account <span class="dd-chev">⌄</span></button>
                  <div class="dd-menu" id="ddMenu">
                    <a href="#">👤 Profile</a>
                    <a href="#">⚙️ Settings</a>
                    <a href="#">💳 Billing</a>
                    <a href="#" class="danger">🚪 Sign out</a>
                  </div>
                </div>
                """,
            Css = """
                * { box-sizing: border-box; }
                body { margin: 0; font-family: 'Segoe UI', system-ui, sans-serif; background: #0f1420; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                .dd { position: relative; }
                .dd-trigger { display: flex; align-items: center; gap: 8px; border: 1px solid rgba(255,255,255,.12); background: rgba(255,255,255,.06); color: #e2e8f0; font-size: 14px; font-weight: 600; padding: 11px 18px; border-radius: 12px; cursor: pointer; transition: background .2s, border-color .2s; }
                .dd-trigger:hover { background: rgba(255,255,255,.1); border-color: rgba(99,102,241,.6); }
                .dd-chev { transition: transform .3s cubic-bezier(.22,1,.36,1); color: #818cf8; }
                .dd.open .dd-chev { transform: rotate(180deg); }
                .dd-menu { position: absolute; top: calc(100% + 8px); left: 0; min-width: 180px; background: #171e2e; border: 1px solid rgba(255,255,255,.09); border-radius: 14px; padding: 6px; box-shadow: 0 20px 50px rgba(0,0,0,.5); opacity: 0; transform: translateY(-6px) scale(.96); transform-origin: top; pointer-events: none; transition: opacity .22s ease, transform .22s cubic-bezier(.22,1,.36,1); }
                .dd.open .dd-menu { opacity: 1; transform: translateY(0) scale(1); pointer-events: auto; }
                .dd-menu a { display: block; padding: 10px 14px; border-radius: 10px; color: #cbd5e1; font-size: 13px; text-decoration: none; transition: background .15s, color .15s; }
                .dd-menu a:hover { background: rgba(99,102,241,.18); color: #fff; }
                .dd-menu a.danger { color: #f87171; }
                .dd-menu a.danger:hover { background: rgba(248,113,113,.12); }
                """,
            Js = """
                const dd = document.getElementById('dd');
                document.getElementById('ddTrigger').addEventListener('click', e => {
                  e.stopPropagation();
                  dd.classList.toggle('open');
                });
                document.addEventListener('click', e => { if (!dd.contains(e.target)) dd.classList.remove('open'); });
                document.querySelectorAll('.dd-menu a').forEach(a => a.addEventListener('click', () => dd.classList.remove('open')));
                """
        }
    };
}
