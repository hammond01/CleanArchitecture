/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    extend: {
      colors: {
        ink: '#0f172a',
        mist: '#f8fafc',
        ember: '#f97316',
        ocean: '#0ea5e9',
      },
    },
  },
  plugins: [],
}
