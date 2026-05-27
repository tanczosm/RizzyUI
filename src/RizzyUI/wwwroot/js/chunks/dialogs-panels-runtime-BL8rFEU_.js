//#region src/js/runtime/a11y/focusable.js
var FOCUSABLE_SELECTOR = [
	"a[href]",
	"area[href]",
	"button",
	"input",
	"select",
	"textarea",
	"summary",
	"iframe",
	"object",
	"embed",
	"[contenteditable]:not([contenteditable=\"false\"])",
	"[tabindex]"
].join(",");
function isElementHiddenByStyle(element) {
	const getComputedStyleFn = (element?.ownerDocument?.defaultView ?? globalThis)?.getComputedStyle;
	if (typeof getComputedStyleFn !== "function") return false;
	const style = getComputedStyleFn(element);
	return style.display === "none" || style.visibility === "hidden" || style.visibility === "collapse" || style.pointerEvents === "none";
}
function isAriaOrInertHidden(element, root) {
	let current = element;
	while (current && current !== root?.parentElement) {
		if (current.getAttribute?.("aria-hidden") === "true" || current.hasAttribute?.("inert")) return true;
		current = current.parentElement;
	}
	return false;
}
function isDisabled(element) {
	if ("disabled" in element && element.disabled) return true;
	if (element.getAttribute?.("aria-disabled") === "true") return true;
	return false;
}
function isNaturallyFocusable(element) {
	const tagName = element.tagName?.toLowerCase();
	if (!tagName) return false;
	if (tagName === "a" || tagName === "area") return !!element.getAttribute?.("href");
	if (tagName === "input") return element.type !== "hidden";
	if ([
		"button",
		"select",
		"textarea",
		"summary",
		"iframe",
		"object",
		"embed"
	].includes(tagName)) return true;
	if (element.isContentEditable) return true;
	return false;
}
/**
* Returns whether an element can receive focus programmatically.
* This excludes disabled controls, inert/aria-hidden branches, and elements hidden via CSS.
* @param {Element | null | undefined} element candidate DOM element.
* @returns {boolean} true when the element is considered focusable.
*/
function isFocusable(element) {
	if (!element || element.nodeType !== 1) return false;
	if (isDisabled(element) || isElementHiddenByStyle(element)) return false;
	if (isAriaOrInertHidden(element)) return false;
	const tabIndexAttr = element.getAttribute?.("tabindex");
	if (tabIndexAttr !== null) {
		const parsed = Number.parseInt(tabIndexAttr, 10);
		if (!Number.isNaN(parsed)) return parsed >= -1;
	}
	return isNaturallyFocusable(element);
}
/**
* Collects focusable descendants within a root container.
* The returned list is DOM-order, and never includes nodes outside the provided root.
* @param {Element | DocumentFragment | null | undefined} root root container to search.
* @returns {Element[]} focusable descendants within root.
*/
function getFocusableElements(root) {
	if (!root || typeof root.querySelectorAll !== "function") return [];
	return Array.from(root.querySelectorAll(FOCUSABLE_SELECTOR)).filter((element) => root.contains?.(element) && isFocusable(element));
}
//#endregion
//#region src/js/runtime/a11y/focusScope.js
var scopeStack = [];
function resolveElement(container, value) {
	if (!value) return null;
	if (typeof value === "string") return container.querySelector?.(value) ?? null;
	return value?.nodeType === 1 ? value : null;
}
function focusElement(target) {
	if (!target || typeof target.focus !== "function") return false;
	target.focus();
	return true;
}
function getTopScope() {
	return scopeStack[scopeStack.length - 1] ?? null;
}
function removeFromStack(scope) {
	const index = scopeStack.lastIndexOf(scope);
	if (index >= 0) scopeStack.splice(index, 1);
}
/**
* Creates a focus scope that traps tab navigation inside a container.
*
* @param {Element} container Root modal container.
* @param {{ initialFocus?: string|Element, fallbackFocus?: string|Element, throwOnNoFocusable?: boolean }} [options]
* @returns {{ activate: () => Element | null, deactivate: () => Element | null, isActive: () => boolean }} Scope API.
*/
function createFocusScope(container, options = {}) {
	if (!container || container.nodeType !== 1) throw new Error("[RizzyUI] createFocusScope requires a valid container element.");
	const config = {
		initialFocus: options.initialFocus ?? null,
		fallbackFocus: options.fallbackFocus ?? null,
		throwOnNoFocusable: options.throwOnNoFocusable ?? false
	};
	let active = false;
	let previouslyFocused = null;
	function focusInitialTarget() {
		const explicitInitial = resolveElement(container, config.initialFocus);
		if (explicitInitial && isFocusable(explicitInitial)) {
			focusElement(explicitInitial);
			return explicitInitial;
		}
		const tabbables = getFocusableElements(container);
		if (tabbables.length) {
			focusElement(tabbables[0]);
			return tabbables[0];
		}
		if (config.throwOnNoFocusable) throw new Error("[RizzyUI] Focus scope activation failed: no focusable elements found in container.");
		const fallback = resolveElement(container, config.fallbackFocus) ?? container;
		if (!isFocusable(fallback) && fallback !== container) throw new Error("[RizzyUI] Focus scope fallbackFocus must resolve to a focusable element.");
		focusElement(fallback);
		return fallback;
	}
	function onKeyDown(event) {
		if (!active || getTopScope() !== api || event.key !== "Tab") return;
		const candidates = getFocusableElements(container).filter((el) => el.getAttribute?.("tabindex") !== "-1");
		if (!candidates.length) {
			event.preventDefault();
			return;
		}
		const first = candidates[0];
		const last = candidates[candidates.length - 1];
		const activeElement = container.ownerDocument?.activeElement;
		if (event.shiftKey) {
			if (activeElement === first || !container.contains(activeElement)) {
				event.preventDefault();
				focusElement(last);
			}
			return;
		}
		if (activeElement === last || !container.contains(activeElement)) {
			event.preventDefault();
			focusElement(first);
		}
	}
	const api = {
		activate() {
			if (active) return container.ownerDocument?.activeElement ?? null;
			previouslyFocused = container.ownerDocument?.activeElement ?? null;
			active = true;
			scopeStack.push(api);
			container.addEventListener("keydown", onKeyDown, true);
			return focusInitialTarget();
		},
		deactivate() {
			if (!active) return null;
			active = false;
			container.removeEventListener("keydown", onKeyDown, true);
			removeFromStack(api);
			if (previouslyFocused && previouslyFocused.isConnected && isFocusable(previouslyFocused)) {
				focusElement(previouslyFocused);
				return previouslyFocused;
			}
			const fallback = resolveElement(container, config.fallbackFocus) ?? container.ownerDocument?.body ?? null;
			focusElement(fallback);
			return fallback;
		},
		isActive() {
			return active;
		}
	};
	return api;
}
//#endregion
//#region src/js/runtime/a11y/dismissableLayer.js
var layerStack = [];
var listenersAttached = false;
function getTopLayer() {
	return layerStack[layerStack.length - 1] ?? null;
}
function isNode(value) {
	return !!value && typeof value === "object" && value.nodeType === 1;
}
function getEventPath(event) {
	if (typeof event.composedPath === "function") return event.composedPath();
	const path = [];
	let current = event.target ?? null;
	while (current) {
		path.push(current);
		current = current.parentElement ?? null;
	}
	const doc = event.target?.ownerDocument ?? document;
	path.push(doc);
	path.push(doc.defaultView ?? window);
	return path;
}
function isEventOutsideLayer(event, layer) {
	const root = layer.root;
	if (!root || !isNode(root)) return true;
	if (getEventPath(event).includes(root)) return false;
	return !root.contains(event.target);
}
function emitDismissEvent(layer, reason, originalEvent) {
	const dismissEvent = new CustomEvent("rz:dismiss", {
		bubbles: true,
		cancelable: true,
		detail: {
			reason,
			layerId: layer.id,
			originalEvent
		}
	});
	layer.root.dispatchEvent(dismissEvent);
	return dismissEvent;
}
function runDismiss(layer, reason, originalEvent) {
	const dismissEvent = emitDismissEvent(layer, reason, originalEvent);
	if (dismissEvent.defaultPrevented || originalEvent.defaultPrevented) return;
	layer.onDismiss({
		reason,
		originalEvent,
		dismissEvent
	});
}
function onDocumentKeyDown(event) {
	if (event.key !== "Escape" || event.defaultPrevented) return;
	const layer = getTopLayer();
	if (!layer) return;
	if (typeof layer.onEscape === "function") layer.onEscape(event);
	if (event.defaultPrevented) return;
	runDismiss(layer, "escape", event);
}
function onDocumentPointerDown(event) {
	const layer = getTopLayer();
	if (!layer || event.defaultPrevented) return;
	if (!isEventOutsideLayer(event, layer)) return;
	if (typeof layer.onOutsidePointer === "function") layer.onOutsidePointer(event);
	if (event.defaultPrevented) return;
	runDismiss(layer, "outside-pointer", event);
}
function onDocumentFocusIn(event) {
	const layer = getTopLayer();
	if (!layer || !layer.dismissOnOutsideFocus || event.defaultPrevented) return;
	if (!isEventOutsideLayer(event, layer)) return;
	if (typeof layer.onOutsideFocus === "function") layer.onOutsideFocus(event);
	if (event.defaultPrevented) return;
	runDismiss(layer, "outside-focus", event);
}
function attachListeners() {
	if (listenersAttached || typeof document === "undefined") return;
	document.addEventListener("keydown", onDocumentKeyDown, true);
	document.addEventListener("pointerdown", onDocumentPointerDown, true);
	document.addEventListener("focusin", onDocumentFocusIn, true);
	listenersAttached = true;
}
function detachListeners() {
	if (!listenersAttached || typeof document === "undefined") return;
	document.removeEventListener("keydown", onDocumentKeyDown, true);
	document.removeEventListener("pointerdown", onDocumentPointerDown, true);
	document.removeEventListener("focusin", onDocumentFocusIn, true);
	listenersAttached = false;
}
/**
* Registers a dismissable overlay layer in a shared stack.
* Only the top-most layer receives escape/outside dismissal interactions.
*
* @param {object} options Registration options.
* @param {HTMLElement} options.root Root element for the layer.
* @param {(context: { reason: string, originalEvent: Event, dismissEvent: CustomEvent }) => void} options.onDismiss Called when dismissal is confirmed.
* @param {(event: KeyboardEvent) => void} [options.onEscape] Optional hook before escape dismissal.
* @param {(event: PointerEvent) => void} [options.onOutsidePointer] Optional hook before outside pointer dismissal.
* @param {(event: FocusEvent) => void} [options.onOutsideFocus] Optional hook before outside focus dismissal.
* @param {boolean} [options.dismissOnOutsideFocus=false] Whether outside focus should dismiss the top layer.
* @param {string} [options.id] Stable optional layer identifier emitted in rz:dismiss details.
* @returns {() => void} Unregister function.
*/
function registerDismissableLayer(options) {
	if (!options || !isNode(options.root)) throw new Error("registerDismissableLayer requires a valid root element.");
	if (typeof options.onDismiss !== "function") throw new Error("registerDismissableLayer requires an onDismiss callback.");
	const layer = {
		id: options.id ?? `layer-${Date.now()}-${Math.random().toString(36).slice(2)}`,
		root: options.root,
		onDismiss: options.onDismiss,
		onEscape: options.onEscape,
		onOutsidePointer: options.onOutsidePointer,
		onOutsideFocus: options.onOutsideFocus,
		dismissOnOutsideFocus: options.dismissOnOutsideFocus === true
	};
	layerStack.push(layer);
	attachListeners();
	let unregistered = false;
	return function unregisterDismissableLayer() {
		if (unregistered) return;
		unregistered = true;
		const index = layerStack.lastIndexOf(layer);
		if (index !== -1) layerStack.splice(index, 1);
		if (layerStack.length === 0) detachListeners();
	};
}
/**
* Creates a dismissable-layer manager API bound to the shared module stack.
*
* @returns {{ registerLayer: typeof registerDismissableLayer }}
*/
function createDismissableLayer() {
	return { registerLayer: registerDismissableLayer };
}
//#endregion
//#region src/js/lib/components/rzModal.js
function rzModal() {
	return {
		modalOpen: false,
		eventTriggerName: "",
		closeEventName: "rz:modal-close",
		closeOnEscape: true,
		closeOnClickOutside: true,
		modalId: "",
		bodyId: "",
		footerId: "",
		nonce: "",
		_openListener: null,
		_closeEventListener: null,
		_focusScope: null,
		_unregisterLayer: null,
		_lastInvoker: null,
		init() {
			this.modalId = this.$el.dataset.modalId || "";
			this.bodyId = this.$el.dataset.bodyId || "";
			this.footerId = this.$el.dataset.footerId || "";
			this.nonce = this.$el.dataset.nonce || "";
			this.eventTriggerName = this.$el.dataset.eventTriggerName || "";
			this.closeEventName = this.$el.dataset.closeEventName || this.closeEventName;
			this.closeOnEscape = this.$el.dataset.closeOnEscape !== "false";
			this.closeOnClickOutside = this.$el.dataset.closeOnClickOutside !== "false";
			this.$el.dispatchEvent(new CustomEvent("rz:modal-initialized", {
				detail: {
					modalId: this.modalId,
					bodyId: this.bodyId,
					footerId: this.footerId
				},
				bubbles: true
			}));
			if (this.eventTriggerName) {
				this._openListener = (e) => this.openModal(e);
				window.addEventListener(this.eventTriggerName, this._openListener);
			}
			this._closeEventListener = () => {
				if (this.modalOpen) this.closeModalInternally("event");
			};
			window.addEventListener(this.closeEventName, this._closeEventListener);
			this.$watch("modalOpen", (value) => {
				const currentWidth = document.body.offsetWidth;
				document.body.classList.toggle("overflow-hidden", value);
				const scrollBarWidth = document.body.offsetWidth - currentWidth;
				document.body.style.setProperty("--page-scrollbar-width", `${scrollBarWidth}px`);
				if (value) this.$nextTick(() => {
					this._activateAccessibility();
					this.$el.dispatchEvent(new CustomEvent("rz:modal-after-open", {
						detail: { modalId: this.modalId },
						bubbles: true
					}));
				});
				else {
					this._deactivateAccessibility();
					this.$nextTick(() => {
						this.$el.dispatchEvent(new CustomEvent("rz:modal-after-close", {
							detail: { modalId: this.modalId },
							bubbles: true
						}));
					});
				}
			});
		},
		destroy() {
			if (this._openListener && this.eventTriggerName) window.removeEventListener(this.eventTriggerName, this._openListener);
			if (this._closeEventListener) window.removeEventListener(this.closeEventName, this._closeEventListener);
			this._deactivateAccessibility();
			document.body.classList.remove("overflow-hidden");
			document.body.style.setProperty("--page-scrollbar-width", `0px`);
		},
		_resolveDialogElement() {
			return this.$el.querySelector("[role=\"dialog\"], [role=\"alertdialog\"], [data-modal-panel=\"true\"]");
		},
		_ensureAriaRelationships(dialogElement) {
			const labelledBy = dialogElement.getAttribute("aria-labelledby");
			const describedBy = dialogElement.getAttribute("aria-describedby");
			if (labelledBy && !document.getElementById(labelledBy)) dialogElement.removeAttribute("aria-labelledby");
			if (describedBy && !document.getElementById(describedBy)) dialogElement.removeAttribute("aria-describedby");
		},
		_activateAccessibility() {
			const dialogElement = this._resolveDialogElement();
			if (!dialogElement) return;
			this._ensureAriaRelationships(dialogElement);
			this._focusScope?.deactivate();
			this._focusScope = createFocusScope(dialogElement, { fallbackFocus: dialogElement });
			this._focusScope.activate();
			this._unregisterLayer?.();
			this._unregisterLayer = createDismissableLayer().registerLayer({
				id: this.modalId || void 0,
				root: dialogElement,
				onEscape: (event) => {
					if (!this.closeOnEscape) event.preventDefault();
				},
				onOutsidePointer: (event) => {
					if (!this.closeOnClickOutside) event.preventDefault();
				},
				onDismiss: ({ reason }) => this.closeModalInternally(reason)
			});
		},
		_deactivateAccessibility() {
			if (this._unregisterLayer) {
				this._unregisterLayer();
				this._unregisterLayer = null;
			}
			if (this._focusScope) {
				const restored = this._focusScope.deactivate();
				this._focusScope = null;
				if ((!restored || restored === document.body) && this._lastInvoker?.isConnected) this._lastInvoker.focus();
			}
		},
		openModal(event = null) {
			const invoker = event?.target ?? document.activeElement;
			if (invoker?.nodeType === 1) this._lastInvoker = invoker;
			const beforeOpenEvent = new CustomEvent("rz:modal-before-open", {
				detail: {
					modalId: this.modalId,
					originalEvent: event
				},
				bubbles: true,
				cancelable: true
			});
			this.$el.dispatchEvent(beforeOpenEvent);
			if (!beforeOpenEvent.defaultPrevented) this.modalOpen = true;
		},
		closeModalInternally(reason = "unknown") {
			const beforeCloseEvent = new CustomEvent("rz:modal-before-close", {
				detail: {
					modalId: this.modalId,
					reason
				},
				bubbles: true,
				cancelable: true
			});
			this.$el.dispatchEvent(beforeCloseEvent);
			if (!beforeCloseEvent.defaultPrevented) {
				this.modalOpen = false;
				document.body.classList.remove("overflow-hidden");
				document.body.style.setProperty("--page-scrollbar-width", `0px`);
			}
		},
		closeModal() {
			this.closeModalInternally("button");
		},
		handleClickOutside() {
			if (this.closeOnClickOutside) this.closeModalInternally("backdrop");
		}
	};
}
//#endregion
//#region src/js/lib/components/rzSheet.js
function rzSheet() {
	return {
		open: false,
		modal: true,
		dismissOnOutsideClick: true,
		focusScope: null,
		unregisterLayer: null,
		init() {
			this.open = this.$el.dataset.defaultOpen === "true";
			this.modal = this.$el.dataset.modal !== "false";
			this.dismissOnOutsideClick = this.$el.dataset.dismissOnOutsideClick !== "false";
			this.$watch("open", (isOpen) => {
				if (isOpen) {
					this.registerInteractions();
					this.applyClosedState(false);
					return;
				}
				this.teardownInteractions();
				this.applyClosedState(true);
			});
			this.applyClosedState(!this.open);
			if (this.open) this.registerInteractions();
		},
		registerInteractions() {
			const panel = this.getPanel();
			if (!panel) return;
			if (!this.unregisterLayer) this.unregisterLayer = registerDismissableLayer({
				root: panel,
				onDismiss: ({ reason }) => {
					if (!this.open) return;
					if (reason === "outside-pointer" && !this.dismissOnOutsideClick) return;
					this.close();
				}
			});
			if (this.modal && !this.focusScope) {
				this.focusScope = createFocusScope(panel, { fallbackFocus: panel });
				this.focusScope.activate();
			}
		},
		teardownInteractions() {
			if (this.focusScope) {
				this.focusScope.deactivate();
				this.focusScope = null;
			}
			if (this.unregisterLayer) {
				this.unregisterLayer();
				this.unregisterLayer = null;
			}
		},
		applyClosedState(closed) {
			const panel = this.getPanel();
			if (!panel) return;
			panel.setAttribute("aria-hidden", closed ? "true" : "false");
		},
		getPanel() {
			return this.$root.querySelector("[data-rz-sheet-panel]");
		},
		toggle() {
			this.open = !this.open;
		},
		close() {
			this.open = false;
		},
		show() {
			this.open = true;
		},
		state() {
			return this.open ? "open" : "closed";
		}
	};
}
//#endregion
//#region src/js/lib/components/rzSidebar.js
function rzSidebar() {
	return {
		open: true,
		openMobile: false,
		isMobile: false,
		collapsible: "offcanvas",
		shortcut: "b",
		cookieName: "sidebar_state",
		mobileBreakpoint: 768,
		init() {
			this.collapsible = this.$el.dataset.collapsible || "offcanvas";
			this.shortcut = this.$el.dataset.shortcut || "b";
			this.cookieName = this.$el.dataset.cookieName || "sidebar_state";
			this.mobileBreakpoint = parseInt(this.$el.dataset.mobileBreakpoint) || 768;
			const defaultOpen = this.$el.dataset.defaultOpen === "true";
			const savedState = this.cookieName ? document.cookie.split("; ").find((row) => row.startsWith(`${this.cookieName}=`))?.split("=")[1] : null;
			this.open = savedState !== null && savedState !== void 0 ? savedState === "true" : defaultOpen;
			this.checkIfMobile();
			window.addEventListener("keydown", (e) => {
				if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === this.shortcut.toLowerCase()) {
					e.preventDefault();
					this.toggle();
				}
			});
			this.$watch("open", (value) => {
				if (this.cookieName) document.cookie = `${this.cookieName}=${value}; path=/; max-age=604800`;
			});
			this.$watch("isMobile", () => {
				this.openMobile = false;
			});
		},
		checkIfMobile() {
			this.isMobile = window.innerWidth < this.mobileBreakpoint;
		},
		toggle() {
			if (this.isMobile) this.openMobile = !this.openMobile;
			else this.open = !this.open;
		},
		setOpen(value) {
			this.open = value;
		},
		setOpenMobile(value) {
			this.openMobile = value;
		},
		close() {
			if (this.isMobile) this.openMobile = false;
		},
		isMobileOpen() {
			return this.openMobile;
		},
		get desktopState() {
			return this.open ? "expanded" : "collapsed";
		},
		get state() {
			return this.open ? "expanded" : "collapsed";
		},
		get mobileState() {
			return this.openMobile ? "open" : "closed";
		},
		getCollapsibleAttribute() {
			return this.state === "collapsed" ? this.collapsible : "";
		}
	};
}
//#endregion
export { rzModal, rzSheet, rzSidebar };

//# sourceMappingURL=dialogs-panels-runtime-BL8rFEU_.js.map