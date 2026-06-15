# OPTODEV Member Registration — Frontend

React 19 SPA for the OPTODEV Member Registration platform.

## Pages

| Route | Component | Description |
|-------|-----------|-------------|
| `/` | `LandingPage` | Landing page with "Register as Member" and "Admin Login" CTAs |
| `/register` | `RegistrationWizard` | 5-step member registration wizard |
| `/admin/login` | `LoginPage` | Admin login form |
| `/admin/members` | `AdminMemberList` | Paginated member list with filters |
| `/admin/members/:id` | `AdminMemberDetail` | Read-only member detail view |

## Stack

- **React 19** with TypeScript 6
- **React Router** for page routing
- **React Hook Form** + **Zod** for form state and validation
- **Tailwind CSS v4** for styling (dark mode)
- **Vite 8** for build tooling

## Development

```bash
npm install
npm run dev
```

Runs on http://localhost:5173. API requests proxy to http://localhost:5000.

## Testing

```bash
npm test        # Vitest (component tests)
npm run build   # TypeScript check + production build
```
