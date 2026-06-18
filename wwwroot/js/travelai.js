/* TravelAI - Main JavaScript */
(function () {
    'use strict';

    // ─── Theme ───────────────────────────────────────────────────────
    const html = document.documentElement;
    const ALL_THEMES = ['dark', 'light', 'theme-ocean', 'theme-sunset', 'theme-forest', 'theme-galaxy'];
    const savedTheme = localStorage.getItem('travelai-theme') || 'dark';

    const THEME_COLORS = {
        dark: '#3b82f6', light: '#f59e0b',
        'theme-ocean': '#06b6d4', 'theme-sunset': '#f97316',
        'theme-forest': '#22c55e', 'theme-galaxy': '#a855f7'
    };

    function applyTheme(theme) {
        ALL_THEMES.forEach(t => html.classList.remove(t));
        if (theme === 'light') {
            html.classList.add('light');
        } else {
            html.classList.add('dark');
            if (theme !== 'dark') html.classList.add(theme);
        }
        document.querySelectorAll('.theme-swatch').forEach(s => {
            s.classList.toggle('active', s.getAttribute('data-theme') === theme);
        });
        // Update FAB to reflect current theme
        const color = THEME_COLORS[theme] || '#a855f7';
        const fab = document.getElementById('theme-picker-toggle');
        const dot = document.getElementById('theme-fab-dot');
        if (fab) {
            fab.style.borderColor = color + '60';
            fab.style.color = color;
            fab.style.boxShadow = `0 4px 24px rgba(0,0,0,0.4), 0 0 18px ${color}30`;
        }
        if (dot) dot.style.background = color;
        localStorage.setItem('travelai-theme', theme);
    }

    applyTheme(savedTheme);

    // ─── Theme Picker ─────────────────────────────────────────────────
    const themePickerToggle = document.getElementById('theme-picker-toggle');
    const themeDropdown     = document.getElementById('theme-dropdown');
    let themePickerOpen = false;

    function closeThemePicker() {
        themePickerOpen = false;
        if (themeDropdown) themeDropdown.style.display = 'none';
    }

    themePickerToggle?.addEventListener('click', e => {
        e.stopPropagation();
        if (!themePickerOpen) {
            closeLangPicker();           // close lang if open
            themePickerOpen = true;
            themeDropdown.style.display = 'block';
            if (typeof gsap !== 'undefined') {
                gsap.fromTo(themeDropdown, { opacity: 0, y: 8 }, { opacity: 1, y: 0, duration: 0.2, ease: 'power2.out' });
            }
        } else {
            closeThemePicker();
        }
    });

    document.addEventListener('click', e => {
        if (themePickerOpen && !document.getElementById('theme-picker')?.contains(e.target)) {
            closeThemePicker();
        }
    });

    document.querySelectorAll('.theme-swatch').forEach(swatch => {
        swatch.addEventListener('click', () => {
            const theme = swatch.getAttribute('data-theme');
            applyTheme(theme);
            closeThemePicker();
            const labels = { dark: 'Dark', light: 'Light', 'theme-ocean': 'Ocean', 'theme-sunset': 'Sunset', 'theme-forest': 'Forest', 'theme-galaxy': 'Galaxy' };
            showToast(`Theme: ${labels[theme] || theme}`);
        });
    });

    // ─── Navbar Scroll ───────────────────────────────────────────────
    const navbar = document.getElementById('navbar');
    let lastScroll = 0;
    window.addEventListener('scroll', () => {
        const y = window.scrollY;
        if (navbar) {
            navbar.style.background = y > 60
                ? (html.classList.contains('light') ? 'rgba(248,250,252,0.98)' : 'rgba(3,7,18,0.97)')
                : '';
            navbar.style.boxShadow = y > 60 ? '0 4px 30px rgba(0,0,0,0.25)' : '';
        }
        lastScroll = y;
    }, { passive: true });

    // ─── Mobile Menu ─────────────────────────────────────────────────
    const mobileBtn = document.getElementById('mobile-menu-btn');
    const mobileMenu = document.getElementById('mobile-menu');
    const hamburgerIcon = document.getElementById('hamburger-icon');
    let mobileOpen = false;

    mobileBtn?.addEventListener('click', () => {
        mobileOpen = !mobileOpen;
        if (mobileOpen) {
            mobileMenu.style.display = 'block';
            hamburgerIcon.className = 'fa-solid fa-xmark text-gray-300';
            if (typeof gsap !== 'undefined') {
                gsap.fromTo(mobileMenu, { opacity: 0, y: -10 }, { opacity: 1, y: 0, duration: 0.25, ease: 'power2.out' });
            }
        } else {
            mobileMenu.style.display = 'none';
            hamburgerIcon.className = 'fa-solid fa-bars text-gray-300';
        }
    });

    // Close mobile menu on outside click
    document.addEventListener('click', e => {
        if (mobileOpen && navbar && !navbar.contains(e.target)) {
            mobileOpen = false;
            mobileMenu.style.display = 'none';
            hamburgerIcon.className = 'fa-solid fa-bars text-gray-300';
        }
    });

    // ─── GSAP Scroll Animations ──────────────────────────────────────
    if (typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
        gsap.registerPlugin(ScrollTrigger);

        // Hero entrance — CSS already hides these; GSAP animates TO visible
        const heroTl = gsap.timeline({ delay: 0.15 });
        heroTl
            .to('.hero-badge',    { opacity: 1, y: 0, duration: 0.6,  ease: 'power3.out' })
            .to('.hero-title',    { opacity: 1, y: 0, duration: 0.75, ease: 'power3.out' }, '-=0.35')
            .to('.hero-subtitle', { opacity: 1, y: 0, duration: 0.6,  ease: 'power3.out' }, '-=0.45')
            .to('.hero-search',   { opacity: 1, y: 0, duration: 0.7,  ease: 'power3.out' }, '-=0.4')
            .to('.hero-stats',    { opacity: 1, y: 0, duration: 0.5,  ease: 'power3.out' }, '-=0.35');

        // Scroll reveals — animate TO visible (CSS sets initial hidden state)
        gsap.utils.toArray('.reveal-up').forEach(el => {
            gsap.to(el, {
                scrollTrigger: { trigger: el, start: 'top 88%', toggleActions: 'play none none none' },
                opacity: 1, y: 0, duration: 0.75, ease: 'power3.out'
            });
        });

        gsap.utils.toArray('.reveal-left').forEach(el => {
            gsap.to(el, {
                scrollTrigger: { trigger: el, start: 'top 88%', toggleActions: 'play none none none' },
                opacity: 1, x: 0, duration: 0.75, ease: 'power3.out'
            });
        });

        gsap.utils.toArray('.reveal-right').forEach(el => {
            gsap.to(el, {
                scrollTrigger: { trigger: el, start: 'top 88%', toggleActions: 'play none none none' },
                opacity: 1, x: 0, duration: 0.75, ease: 'power3.out'
            });
        });

        // Stagger card grids
        gsap.utils.toArray('.stagger-cards').forEach(container => {
            gsap.to(container.querySelectorAll('.stagger-card'), {
                scrollTrigger: { trigger: container, start: 'top 85%', toggleActions: 'play none none none' },
                opacity: 1, y: 0, duration: 0.65, stagger: 0.1, ease: 'power3.out'
            });
        });

        // Number counters
        document.querySelectorAll('.count-up').forEach(el => {
            const target = parseInt(el.getAttribute('data-target'), 10);
            const suffix = el.getAttribute('data-suffix') || '';
            ScrollTrigger.create({
                trigger: el, start: 'top 90%', once: true,
                onEnter() {
                    gsap.to({ val: 0 }, {
                        val: target, duration: 2.2, ease: 'power2.out',
                        onUpdate() { el.textContent = Math.round(this.targets()[0].val).toLocaleString() + suffix; }
                    });
                }
            });
        });
    }

    // ─── Hero Particles ──────────────────────────────────────────────
    const particleContainer = document.getElementById('hero-particles');
    if (particleContainer) {
        for (let i = 0; i < 45; i++) {
            const p = document.createElement('div');
            p.className = 'hero-particle';
            p.style.cssText = `
                left:${Math.random() * 100}%;
                top:${Math.random() * 100}%;
                width:${Math.random() * 3 + 1}px;
                height:${Math.random() * 3 + 1}px;
                animation-delay:${Math.random() * 10}s;
                animation-duration:${Math.random() * 7 + 6}s;
                opacity:${Math.random() * 0.5 + 0.1}`;
            particleContainer.appendChild(p);
        }
    }

    // ─── Hero Search ─────────────────────────────────────────────────
    const heroSearchBtn = document.getElementById('hero-search-btn');
    const heroDestInput = document.getElementById('hero-dest');
    const heroBudgetInput = document.getElementById('hero-budget');
    const heroDaysInput = document.getElementById('hero-days');

    heroSearchBtn?.addEventListener('click', () => {
        const dest = heroDestInput?.value.trim();
        if (dest) {
            window.location.href = `/Countries?search=${encodeURIComponent(dest)}`;
        } else {
            showToast('Enter a destination to plan your trip!');
            heroDestInput?.focus();
        }
    });

    // Allow Enter key in hero inputs
    [heroDestInput, heroBudgetInput, heroDaysInput].forEach(input => {
        input?.addEventListener('keydown', e => { if (e.key === 'Enter') heroSearchBtn?.click(); });
    });

    // ─── Chat Widget ─────────────────────────────────────────────────
    const chatToggle = document.getElementById('chat-toggle');
    const chatPanel  = document.getElementById('chat-panel');
    const chatClose  = document.getElementById('chat-close');
    const chatInput  = document.getElementById('chat-input');
    const chatSend   = document.getElementById('chat-send');
    const chatMsgs   = document.getElementById('chat-messages');
    let chatOpen = false;

    function openChat() {
        chatOpen = true;
        chatPanel.style.display = 'flex';
        if (typeof gsap !== 'undefined') {
            gsap.fromTo(chatPanel,
                { opacity: 0, y: 25, scale: 0.94 },
                { opacity: 1, y: 0, scale: 1, duration: 0.35, ease: 'back.out(1.4)' });
        }
        chatInput?.focus();
    }

    function closeChat() {
        chatOpen = false;
        if (typeof gsap !== 'undefined') {
            gsap.to(chatPanel, {
                opacity: 0, y: 20, scale: 0.95, duration: 0.25, ease: 'power2.in',
                onComplete: () => { chatPanel.style.display = 'none'; }
            });
        } else {
            chatPanel.style.display = 'none';
        }
    }

    chatToggle?.addEventListener('click', () => chatOpen ? closeChat() : openChat());
    chatClose?.addEventListener('click', closeChat);

    function appendBubble(role, content) {
        const bubble = document.createElement('div');
        bubble.className = `chat-bubble ${role}`;
        bubble.innerHTML = `<p>${escapeHtml(content)}</p>`;
        chatMsgs.appendChild(bubble);
        chatMsgs.scrollTop = chatMsgs.scrollHeight;
        if (typeof gsap !== 'undefined') {
            gsap.from(bubble, { opacity: 0, y: 12, duration: 0.3, ease: 'power2.out' });
        }
        return bubble;
    }

    function showTyping() {
        const b = document.createElement('div');
        b.className = 'chat-bubble assistant';
        b.id = 'typing-indicator';
        b.innerHTML = '<span class="typing-dots"><span>.</span><span>.</span><span>.</span></span>';
        chatMsgs.appendChild(b);
        chatMsgs.scrollTop = chatMsgs.scrollHeight;
    }

    async function sendChatMessage() {
        const text = chatInput?.value.trim();
        if (!text || chatSend?.disabled) return;
        chatInput.value = '';
        appendBubble('user', text);
        showTyping();
        chatSend.disabled = true;

        try {
            const res = await fetch('/Chat/Send', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-CSRF-TOKEN': getAntiForgeryToken(),
                    'RequestVerificationToken': getAntiForgeryToken()
                },
                body: JSON.stringify(text)
            });
            document.getElementById('typing-indicator')?.remove();
            if (res.ok) {
                const data = await res.json();
                appendBubble('assistant', data.content);
            } else {
                appendBubble('assistant', 'Sorry, something went wrong. Please try again.');
            }
        } catch {
            document.getElementById('typing-indicator')?.remove();
            appendBubble('assistant', 'Connection error. Please check your connection and try again.');
        } finally {
            chatSend.disabled = false;
            chatInput?.focus();
        }
    }

    chatSend?.addEventListener('click', sendChatMessage);
    chatInput?.addEventListener('keydown', e => {
        if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); sendChatMessage(); }
    });

    // ─── Language Switcher ───────────────────────────────────────────
    const langToggle   = document.getElementById('lang-toggle');
    const langDropdown = document.getElementById('lang-dropdown');
    let langOpen = false;

    function closeLangPicker() {
        langOpen = false;
        if (langDropdown) langDropdown.style.display = 'none';
    }

    langToggle?.addEventListener('click', e => {
        e.stopPropagation();
        if (!langOpen) {
            closeThemePicker();          // close theme if open
            langOpen = true;
            langDropdown.style.display = 'block';
            if (typeof gsap !== 'undefined') {
                gsap.fromTo(langDropdown, { opacity: 0, y: 8 }, { opacity: 1, y: 0, duration: 0.2, ease: 'power2.out' });
            }
        } else {
            closeLangPicker();
        }
    });

    document.addEventListener('click', e => {
        if (langOpen && !document.getElementById('lang-switcher')?.contains(e.target)) {
            closeLangPicker();
        }
    });

    document.querySelectorAll('.lang-option').forEach(opt => {
        opt.addEventListener('click', e => {
            e.preventDefault();
            document.querySelectorAll('.lang-option').forEach(o => o.classList.remove('active'));
            opt.classList.add('active');
            closeLangPicker();
            const lang = opt.dataset.lang;
            document.documentElement.setAttribute('lang', lang);
            document.documentElement.setAttribute('dir', lang === 'ar' ? 'rtl' : 'ltr');
            localStorage.setItem('travelai-lang', lang);
            if (typeof applyTranslations === 'function') applyTranslations(lang);
            showToast(`Language switched to ${opt.querySelector('span:last-child')?.textContent}`);
        });
    });

    // Restore saved language
    const savedLang = localStorage.getItem('travelai-lang');
    if (savedLang && savedLang !== 'en') {
        document.querySelectorAll('.lang-option').forEach(o => {
            o.classList.toggle('active', o.dataset.lang === savedLang);
        });
        document.documentElement.setAttribute('lang', savedLang);
        document.documentElement.setAttribute('dir', savedLang === 'ar' ? 'rtl' : 'ltr');
        if (typeof applyTranslations === 'function') applyTranslations(savedLang);
    }

    // ─── FAQ Accordion ───────────────────────────────────────────────
    document.querySelectorAll('.faq-question').forEach(q => {
        q.addEventListener('click', () => {
            const item = q.closest('.faq-item');
            const wasOpen = item.classList.contains('open');
            document.querySelectorAll('.faq-item.open').forEach(i => i.classList.remove('open'));
            if (!wasOpen) item.classList.add('open');
        });
    });

    // ─── Toast Notification ──────────────────────────────────────────
    function showToast(msg, duration = 3000) {
        let toast = document.getElementById('app-toast');
        if (!toast) {
            toast = document.createElement('div');
            toast.id = 'app-toast';
            toast.className = 'toast';
            document.body.appendChild(toast);
        }
        toast.textContent = msg;
        toast.classList.add('show');
        clearTimeout(toast._timer);
        toast._timer = setTimeout(() => toast.classList.remove('show'), duration);
    }

    window.showToast = showToast;

    // ─── Trip Planner (AI page) ───────────────────────────────────────
    const plannerForm = document.getElementById('planner-form');
    plannerForm?.addEventListener('submit', async e => {
        e.preventDefault();
        const btn = plannerForm.querySelector('[type=submit]');
        const originalText = btn.innerHTML;
        btn.disabled = true;
        btn.innerHTML = '<i class="fa-solid fa-spinner fa-spin mr-2"></i>Generating your trip...';

        const dest    = document.getElementById('plan-dest')?.value;
        const budget  = document.getElementById('plan-budget')?.value;
        const days    = document.getElementById('plan-days')?.value;
        const style   = document.getElementById('plan-style')?.value;
        const people  = document.getElementById('plan-people')?.value;

        await new Promise(r => setTimeout(r, 2000));

        const result = document.getElementById('planner-result');
        if (result) {
            result.style.display = 'block';
            if (typeof gsap !== 'undefined') {
                gsap.from(result, { opacity: 0, y: 30, duration: 0.6, ease: 'power3.out' });
            }
            result.innerHTML = buildItinerary(dest, budget, days, style, people);
        }

        btn.disabled = false;
        btn.innerHTML = originalText;
    });

    function buildItinerary(dest, budget, days, style, people) {
        const perDay = Math.floor((Number(budget) || 500) / (Number(days) || 5));
        return `
            <div class="glass-card p-6 mb-4">
                <div class="flex items-center gap-3 mb-6">
                    <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center">
                        <i class="fa-solid fa-map text-white"></i>
                    </div>
                    <div>
                        <h3 class="text-white font-bold text-lg">${escapeHtml(dest || 'Your Destination')} — ${days || 5}-Day Trip</h3>
                        <p class="text-gray-400 text-sm">AI-Generated Itinerary &bull; $${perDay}/day budget &bull; ${people || 2} person(s)</p>
                    </div>
                </div>
                ${Array.from({ length: Math.min(Number(days) || 5, 5) }, (_, i) => `
                <div class="itinerary-day">
                    <div class="flex items-center gap-2 mb-2">
                        <span class="step-number text-xs">${i + 1}</span>
                        <h4 class="text-white font-semibold">Day ${i + 1}</h4>
                    </div>
                    <p class="text-gray-400 text-sm leading-relaxed">
                        ${getDayPlan(i, style, dest)}
                    </p>
                </div>`).join('')}
                <div class="mt-4 p-4 rounded-xl" style="background:rgba(37,99,235,0.08);border:1px solid rgba(37,99,235,0.2)">
                    <p class="text-blue-300 text-sm"><i class="fa-solid fa-crown mr-2 text-amber-400"></i>
                    <strong>Upgrade to Premium</strong> to get real-time hotel booking, flight prices, and a full exportable PDF itinerary.</p>
                </div>
            </div>`;
    }

    function getDayPlan(i, style, dest) {
        const plans = {
            0: `Arrive and check into your hotel. Take an easy afternoon walk to explore the local neighbourhood. Have dinner at a highly-rated local restaurant.`,
            1: `Morning visit to the top landmark. Afternoon guided tour or museum. Evening rooftop bar for sunset views.`,
            2: `Day trip to a nearby attraction outside the city. Pack a picnic or stop at a local café for lunch.`,
            3: `Explore local markets and street food. Afternoon activity (cooking class, boat tour, or hiking).`,
            4: `Relaxed morning. Last-minute shopping and souvenirs. Head to the airport or enjoy a farewell dinner.`,
        };
        return plans[i] || `Explore more of ${escapeHtml(dest || 'the destination')} at your own pace.`;
    }

    // ─── Utilities ───────────────────────────────────────────────────
    function escapeHtml(str) {
        if (!str) return '';
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }

    function getAntiForgeryToken() {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value
            || document.querySelector('meta[name="__RequestVerificationToken"]')?.content
            || '';
    }

    // ─── GSAP Fallback ───────────────────────────────────────────────
    // If GSAP fails to load or animate, show all content after 4s
    setTimeout(() => {
        if (typeof gsap === 'undefined') {
            document.body.classList.add('gsap-fallback');
        }
    }, 4000);

})();
