// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(() => {
    const loginUserForm = document.querySelector(".login-user-form");
    const loginSection = document.getElementById("loginUserSection");

    if (!loginUserForm || !loginSection) {
        return;
    }

    const hasLoginUser = loginSection.dataset.hasLoginUser === "true";
    const passwordInput = document.getElementById("Password");
    const confirmPasswordInput = document.getElementById("ConfirmPassword");
    const passwordMismatchMessage = document.getElementById("passwordMismatchClientMessage");
    const passwordRequirementStatuses = loginSection.querySelectorAll("[data-password-requirement]");

    const setRequirementStatus = (element, required) => {
        element.textContent = required ? "必須" : "任意";
        element.classList.toggle("field-required", required);
        element.classList.toggle("field-optional", !required);
    };

    const syncPasswordConfirmation = () => {
        if (!passwordInput || !confirmPasswordInput) {
            return;
        }

        const passwordEntered = passwordInput.value.length > 0;
        const confirmationEntered = confirmPasswordInput.value.length > 0;
        const passwordRequired = !hasLoginUser || passwordEntered || confirmationEntered;
        passwordInput.required = passwordRequired;
        confirmPasswordInput.required = passwordRequired;
        passwordRequirementStatuses.forEach(status => setRequirementStatus(status, passwordRequired));

        if ((!passwordEntered && !confirmationEntered)
            || confirmPasswordInput.value === passwordInput.value) {
            confirmPasswordInput.setCustomValidity("");
            passwordMismatchMessage?.classList.add("d-none");
            return;
        }

        confirmPasswordInput.setCustomValidity("パスワードとパスワード確認が一致しません。");
    };

    const clearPasswordInputs = () => {
        if (!passwordInput || !confirmPasswordInput) {
            return;
        }

        passwordInput.value = "";
        confirmPasswordInput.value = "";
    };

    passwordInput?.addEventListener("input", syncPasswordConfirmation);
    confirmPasswordInput?.addEventListener("input", syncPasswordConfirmation);
    loginUserForm.addEventListener("submit", event => {
        if (!passwordInput
            || !confirmPasswordInput
            || passwordInput.value === confirmPasswordInput.value) {
            return;
        }

        event.preventDefault();
        event.stopImmediatePropagation();
        clearPasswordInputs();
        syncPasswordConfirmation();
        confirmPasswordInput.setCustomValidity("パスワードとパスワード確認が一致しません。");
        passwordMismatchMessage?.classList.remove("d-none");
        passwordInput.focus();
    });
    syncPasswordConfirmation();
})();

(() => {
    document.querySelectorAll("[data-password-toggle]").forEach(button => {
        const targetSelector = button.getAttribute("data-password-toggle");
        const input = targetSelector ? document.querySelector(targetSelector) : null;
        if (!(input instanceof HTMLInputElement)) {
            return;
        }

        const baseLabel = button.getAttribute("aria-label") ?? "パスワードを表示";
        const hideLabel = baseLabel.replace("表示", "非表示");

        input.type = "password";
        button.setAttribute("aria-pressed", "false");
        button.setAttribute("aria-label", baseLabel);

        button.addEventListener("click", () => {
            const shouldShow = input.type === "password";
            input.type = shouldShow ? "text" : "password";
            button.setAttribute("aria-pressed", shouldShow.toString());
            button.setAttribute("aria-label", shouldShow ? hideLabel : baseLabel);
            input.focus();
        });
    });
})();

(() => {
    document.querySelectorAll("form").forEach(form => {
        form.addEventListener("submit", event => {
            if (window.jQuery && window.jQuery.fn.valid && !window.jQuery(form).valid()) {
                return;
            }
            if (form.checkValidity && !form.checkValidity()) {
                return;
            }
            if (form.dataset.submitted === "true") {
                event.preventDefault();
                return;
            }

            form.dataset.submitted = "true";
            form.querySelectorAll("button[type='submit']").forEach(button => {
                button.disabled = true;
            });
        });
    });
})();
