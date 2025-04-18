import React from 'react';

const ActorInput = ({ label, value, onChange }) => {
  return (
    <div className="actor-input">
      <label>{label}</label>
      <input
        type="text"
        value={value}
        onChange={onChange}
        placeholder={`Digite o nome do ${label.toLowerCase()}`}
      />
    </div>
  );
};

export default ActorInput;
