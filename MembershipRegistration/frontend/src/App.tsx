import RegistrationWizard from "./components/RegistrationWizard";

export default function App() {
  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 py-4">
          <h1 className="text-xl font-bold text-gray-800">OPTODEV</h1>
        </div>
      </header>
      <main className="py-8">
        <RegistrationWizard />
      </main>
    </div>
  );
}
