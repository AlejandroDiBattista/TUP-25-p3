    function useContactosStorage(contactosIniciales) {
        const [contactos, setContactos] = useState(contactosIniciales);

        useEffect(() => {
            const contactosGuardados = localStorage.getItem("contactos");
            if (contactosGuardados) {
                setContactos(JSON.parse(contactosGuardados));
            }
        }, []);

        useEffect(() => {
            localStorage.setItem("contactos", JSON.stringify(contactos));
        }, [contactos]);

        return [contactos, setContactos];
    }
