import { useState } from 'react';
import './App.css';

const buttons = [
  ['AC', '+/-', '%', '÷'],
  ['7', '8', '9', '×'],
  ['4', '5', '6', '-'],
  ['1', '2', '3', '+'],
  ['0', '.', '=']
];

function App() {
  const [input, setInput] = useState('0');
  const [error, setError] = useState('');

  // Función para formatear números con coma cada 3 dígitos
  const formatNumberWithCommas = (str) => {
    const [integerPart, decimalPart] = str.split('.');
    const sign = integerPart.startsWith('-') ? '-' : '';
    const num = sign ? integerPart.slice(1) : integerPart;

    const formattedInteger = num.replace(/\B(?=(\d{3})+(?!\d))/g, ',');

    return decimalPart !== undefined
      ? sign + formattedInteger + '.' + decimalPart
      : sign + formattedInteger;
  };

  const handleClick = (value) => {
    setError('');

    if (value === 'AC') {
      setInput('0');
      return;
    }

    if (value === '=') {
      try {
        const replaced = input.replace(/×/g, '*').replace(/÷/g, '/');
        let result = eval(replaced).toString();

        if (result.length > 15) result = result.slice(0, 15);
        setInput(result);
      } catch {
        setInput('Error');
      }
      return;
    }

    if (value === '+/-') {
      setInput((prev) =>
        prev.startsWith('-') ? prev.slice(1) : '-' + prev
      );
      return;
    }

    setInput((prev) => {
      if (prev === 'Error') return value;

      const operators = ['+', '-', '×', '÷'];
      const lastChar = prev[prev.length - 1];

      if (operators.includes(value) && operators.includes(lastChar)) {
        return prev.slice(0, -1) + value;
      }

      if (value === '.') {
        const lastNumber = prev.split(/[\+\-\×\÷]/).pop();
        if (lastNumber.includes('.')) return prev;
      }

      const newInput = prev === '0' && !['.', '+', '-', '×', '÷'].includes(value)
        ? value
        : prev + value;

      const lastNumber = newInput.split(/[\+\-\×\÷]/).pop();
      const digitCount = lastNumber.replace(/\D/g, '');
      if (digitCount.length > 15) {
        setError('No es posible introducir más de 15 dígitos por número');
        return prev;
      }

      return newInput;
    });
  };

  // Para mostrar el input formateado con comas, excepto cuando sea "Error"
  const displayValue = input === 'Error' ? input : formatNumberWithCommas(input);

  const getFontSizeClass = () => {
    const digits = input.replace(/[^0-9]/g, '').length;
    if (digits >= 10) return 'display small';
    return 'display normal';
  };

  return (
  <div className="app-wrapper">
    {error && <span className="floating-error">{error}</span>}

    <div className="calculator">
      <div className={getFontSizeClass()}>{displayValue}</div>
      <div className="buttons">
        {buttons.flat().map((btn, i) => (
          <button
            key={i}
            onClick={() => handleClick(btn)}
            className={`btn ${btn === '0' ? 'zero' : ''} ${
              ['÷', '×', '-', '+', '='].includes(btn) ? 'operator' : ''
            } ${['AC', '+/-', '%'].includes(btn) ? 'function' : ''}`}
          >
            {btn}
          </button>
        ))}
      </div>
    </div>
  </div>
);

}

export default App;
