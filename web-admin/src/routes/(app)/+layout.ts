import { redirect } from '@sveltejs/kit';
import { browser } from '$app/environment';
import { auth } from '$lib/stores/auth.svelte';

export const ssr = false; // Force client-side rendering for auth

export const load = async ({ url }: { url: URL }) => {
	if (browser) {
		// Wait for auth to finish loading
		while (auth.isLoading) {
			await new Promise((resolve) => setTimeout(resolve, 10));
		}

		// Redirect to login if not authenticated
		if (!auth.isAuthenticated) {
			throw redirect(307, `/login?redirectTo=${encodeURIComponent(url.pathname)}`);
		}

		// Redirect to change password if required
		if (auth.mustChangePassword && url.pathname !== '/change-password') {
			throw redirect(307, '/change-password');
		}
	}

	return {};
};