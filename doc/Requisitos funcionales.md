 # Requisitos funcionales

 ## RF de Alto Nivel (AN)

 ### AN-01 — Gestión de cliente

 - **AN-01.1:** El sistema debe permitir actualizar los datos personales del cliente.

 ### AN-02 — Gestión de cuentas

 - **AN-02.1:** El sistema debe mostrar el saldo disponible de la cuenta.
 - **AN-02.2:** El sistema debe mostrar el historial completo de movimientos.

 ### AN-03 — Movimientos

 - **AN-03.1:** El sistema debe permitir depósitos.
 - **AN-03.2:** El sistema debe permitir retiros.
 - **AN-03.3:** El sistema debe permitir transferencias entre cuentas de diferentes personas.
 - **AN-03.4:** El sistema debe registrar todo movimiento efectuado.

 ### AN-04 — Gestión de tarjetas

 - **AN-04.1:** Permitir crear tarjetas de crédito.
 <!-- - **AN-04.2:** Permitir bloquear tarjetas de crédito. -->
 - **AN-04.3:** Permitir eliminar tarjetas de crédito.

 ### AN-05 — Intereses

 - **AN-05.1:** El sistema debe calcular y acreditar intereses mensuales en cuentas de ahorros.

 ## RF de Bajo Nivel (BN)

 ### BN-01 — Validaciones

 - **BN-01.1:** Verificar saldo suficiente antes de retiros o transferencias.
 - **BN-01.2:** Para cuentas corrientes permitir sobregiro hasta un límite configurado.
 - **BN-01.3:** El monto máximo permitido por movimiento es de 5000.
 - **BN-01.4:** Validar existencia de cuentas antes de cualquier operación.
 - **BN-01.5:** Permitir movimientos internos donde la cuenta origen y destino sean iguales.

 ### BN-02 — Movimientos

 - **BN-02.1:** Registrar hora, fecha, tipo y monto en cada movimiento.
 - **BN-02.2:** Registrar intereses como movimiento automático.

 ### BN-03 — Seguridad funcional

 - **BN-03.1:** Impedir operaciones con datos incompletos.
 - **BN-03.2:** Validar que las tarjetas existan antes de bloquearlas o eliminarlas.
