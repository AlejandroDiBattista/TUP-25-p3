/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './**/*.{razor,html}',
    './**/(Layout|Pages|Components)/*.{razor,html}', // Include only Layout and Pages folders
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}
