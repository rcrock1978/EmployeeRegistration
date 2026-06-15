import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import React from 'react'
import ReactDOM from 'react-dom'
import './index.css'
import App from './App.tsx'

async function main() {
  if (import.meta.env.DEV) {
    const axe = await import('@axe-core/react')
    await axe.default(React, ReactDOM, 1000)
  }

  createRoot(document.getElementById('root')!).render(
    <StrictMode>
      <App />
    </StrictMode>,
  )
}

main()
