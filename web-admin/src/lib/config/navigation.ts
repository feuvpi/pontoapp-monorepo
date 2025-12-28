import {
	LayoutDashboard,
	Users,
	Clock,
	FileText,
	Settings,
	Calendar,
	MapPin,
	Building2
} from 'lucide-svelte';

export interface NavItem {
	title: string;
	href: string;
	icon: typeof LayoutDashboard;
	badge?: string;
	roles?: string[]; // Se definido, apenas esses roles podem ver
}

export interface NavGroup {
	title?: string;
	items: NavItem[];
}


//Main navigation configuration
export const navigation: NavGroup[] = [
	{
		items: [
			{
				title: 'Dashboard',
				href: '/dashboard',
				icon: LayoutDashboard
			}
		]
	},
	{
		title: 'Gestão',
		items: [
			{
				title: 'Funcionários',
				href: '/employees',
				icon: Users,
				roles: ['Admin', 'HR', 'Manager']
			},
			{
				title: 'Registros de Ponto',
				href: '/time-records',
				icon: Clock,
				roles: ['Admin', 'HR', 'Manager']
			},
			{
				title: 'Relatórios',
				href: '/reports',
				icon: FileText,
				roles: ['Admin', 'HR', 'Manager']
			}
		]
	},
	{
		title: 'Configurações',
		items: [
			{
				title: 'Empresa',
				href: '/settings/company',
				icon: Building2,
				roles: ['Admin']
			},
			{
				title: 'Jornadas',
				href: '/settings/work-schedules',
				icon: Calendar,
				roles: ['Admin']
			},
			{
				title: 'Locais',
				href: '/settings/locations',
				icon: MapPin,
				roles: ['Admin']
			},
			{
				title: 'Sistema',
				href: '/settings',
				icon: Settings,
				roles: ['Admin']
			}
		]
	}
];

/**
 * Filter navigation items based on user role
 */
export function filterNavigationByRole(nav: NavGroup[], userRole?: string): NavGroup[] {
	if (!userRole) return [];

	return nav
		.map((group) => ({
			...group,
			items: group.items.filter((item) => {
				// If no roles defined, item is visible to all
				if (!item.roles) return true;
				// Check if user role is in allowed roles
				return item.roles.includes(userRole);
			})
		}))
		.filter((group) => group.items.length > 0); // Remove empty groups
}