import { format as dateFnsFormat, parseISO, formatDistanceToNow } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { DATE_FORMATS } from './constants';

/**
 * Format date to Brazilian format
 */
export function formatDate(date: Date | string, formatStr: string = DATE_FORMATS.SHORT): string {
	const dateObj = typeof date === 'string' ? parseISO(date) : date;
	return dateFnsFormat(dateObj, formatStr, { locale: ptBR });
}

/**
 * Format date to time only (HH:mm)
 */
export function formatTime(date: Date | string): string {
	return formatDate(date, DATE_FORMATS.TIME);
}

/**
 * Format date to datetime (dd/MM/yyyy HH:mm)
 */
export function formatDateTime(date: Date | string): string {
	return formatDate(date, DATE_FORMATS.DATETIME);
}

/**git 
 * Format relative time (e.g., "há 2 horas")
 */
export function formatRelativeTime(date: Date | string): string {
	const dateObj = typeof date === 'string' ? parseISO(date) : date;
	return formatDistanceToNow(dateObj, { addSuffix: true, locale: ptBR });
}

/**
 * Format duration in minutes to hours and minutes
 * @example formatDuration(125) => "2h 5min"
 */
export function formatDuration(minutes: number): string {
	const hours = Math.floor(minutes / 60);
	const mins = minutes % 60;
	
	if (hours === 0) return `${mins}min`;
	if (mins === 0) return `${hours}h`;
	return `${hours}h ${mins}min`;
}

/**
 * Format CPF (000.000.000-00)
 */
export function formatCPF(cpf: string): string {
	const cleaned = cpf.replace(/\D/g, '');
	return cleaned.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
}

/**
 * Format phone number (BR)
 * @example formatPhone("11987654321") => "(11) 98765-4321"
 */
export function formatPhone(phone: string): string {
	const cleaned = phone.replace(/\D/g, '');
	
	if (cleaned.length === 11) {
		return cleaned.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
	}
	
	if (cleaned.length === 10) {
		return cleaned.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
	}
	
	return phone;
}

/**
 * Capitalize first letter
 */
export function capitalize(str: string): string {
	return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
}

/**
 * Get initials from name
 * @example getInitials("João da Silva") => "JS"
 */
export function getInitials(name: string): string {
	const words = name.trim().split(' ').filter(Boolean);
	
	if (words.length === 1) {
		return words[0].charAt(0).toUpperCase();
	}
	
	return (words[0].charAt(0) + words[words.length - 1].charAt(0)).toUpperCase();
}

/**
 * Truncate text with ellipsis
 */
export function truncate(text: string, maxLength: number): string {
	if (text.length <= maxLength) return text;
	return text.slice(0, maxLength - 3) + '...';
}