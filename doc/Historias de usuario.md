# MÓDULO: Gestión de Cliente  
## HU-01 – Actualizar datos personales  
  
> Como cliente  
> quiero actualizar mis datos personales  
> para mantener mi información correcta y vigente.  
  
### Criterios de Aceptación  
  
- CA1: El sistema debe permitir modificar únicamente datos personales permitidos (nombre, dirección, teléfono, email).  
  
- CA2: El sistema debe validar que los campos obligatorios estén completos.  
  
- CA3: El sistema debe guardar los cambios de forma inmediata.  
  
- CA4: El sistema debe mostrar un mensaje de confirmación cuando los datos hayan sido actualizados correctamente.  
  
# MÓDULO: Gestión de Cuentas  
  
<!-- ## HU-02 – Consultar saldo  
  
> Como cliente  
> quiero consultar el saldo de mi cuenta  
> para conocer el dinero disponible.  
  
### Criterios de Aceptación  
  
- CA1: El sistema debe mostrar el saldo actual de la cuenta.  
  
- CA2: El sistema debe mostrar el monto disponible considerando sobregiro (solo para cuenta corriente).  
  
- CA3: El sistema debe impedir que se muestre información de cuentas inexistentes.   -->
  
<!-- ## HU-03 – Consultar historial de movimientos  
  
> Como cliente  
> quiero consultar mi historial de movimientos  
> para conocer mis transacciones realizadas.  
  
### Criterios de Aceptación  
  
- CA1: El sistema debe mostrar el listado completo de movimientos de la cuenta.  
  
- CA2: Cada movimiento debe mostrar: fecha, tipo (depósito/retiro/transferencia), monto y cuenta origen/destino.  
  
- CA3: Debe permitir filtrar por fechas.  
  
- CA4: Debe impedir el acceso si la cuenta no existe.   -->
  
# MÓDULO: Gestión de Tarjetas de Crédito  
  
## HU-04 – Gestionar tarjetas de crédito (crear, bloquear, eliminar)  
  
> Como cliente  
> quiero gestionar mis tarjetas de crédito  
> para controlar su disponibilidad y uso.  
  
### Criterios de Aceptación  
  
- CA1: El sistema debe permitir crear una tarjeta asociada al cliente.  
  
- CA2: El sistema debe permitir bloquear una tarjeta existente.  
  
- CA3: El sistema debe permitir activar una tarjeta ya bloqueada.  
  
- CA5: El sistema debe mostrar confirmación de cada operación realizada.  
  
# MÓDULO: Movimientos  
  
## HU-05 – Realizar depósito  
  
> Como cliente  
> quiero realizar un depósito en mi cuenta  
> para incrementar mi saldo disponible.  
  
### Criterios de Aceptación  
  
- CA1: El depósito debe incrementar el saldo de la cuenta inmediatamente.  
  
- CA2: No debe existir un límite para el número de depósitos.  
  
- CA3: El sistema debe registrar el depósito como movimiento.  
  
- CA4: El sistema debe impedir depósitos a cuentas inexistentes.  
  
## HU-06 – Realizar retiro  
  
> Como cliente  
> quiero realizar un retiro  
> para obtener dinero de mi cuenta.  
  
### Criterios de Aceptación  
  
- CA1: El sistema debe verificar que el monto del retiro no exceda el saldo disponible.  
  
- CA2: Para cuentas corrientes, se debe permitir saldo negativo solo hasta el límite de sobregiro configurado.  
  
- CA3: Para cuentas de ahorros no se permite dejar saldo negativo.  
  
- CA4: El sistema debe registrar el movimiento.  
  
- CA5: El monto máximo por movimiento no puede exceder 5000.  
  
## HU-07 – Realizar transferencia
  
> Como cliente  
> quiero realizar una transferencia a la cuenta de otra persona  
> para enviar dinero cuando lo necesite.  
  
### Criterios de Aceptación  
  
- CA1: La cuenta origen (del cliente) y la cuenta destino (de un tercero) deben existir.

- CA2: El sistema debe impedir transferencias hacia cuentas inactivas o inexistentes.

- CA3: El monto máximo permitido por transferencia es 5000.

- CA4: El sistema debe verificar que el saldo sea suficiente.
    - Para cuenta corriente, se permite sobregiro hasta el límite configurado.
    - Para cuenta de ahorros, no se permite dejar saldo negativo.

- CA5: El movimiento debe registrarse con fecha, monto, tipo y cuenta destino.

- CA6: El sistema debe mostrar un mensaje de confirmación cuando la transferencia se complete.
# MÓDULO: Reglas Automáticas del Sistema  
  
## HU-08 – Acreditar intereses mensuales en cuenta de ahorros  
  
> Como sistema  
> quiero acreditar intereses mensuales a las cuentas de ahorro  
> para reflejar el rendimiento correspondiente.  
  
### Criterios de Aceptación  
  
- CA1: El sistema debe calcular los intereses usando un porcentaje realista configurado.  
  
- CA2: Los intereses deben sumarse al saldo el primer día de cada mes.  
  
- CA3: El sistema debe registrar esta acreditación como movimiento.  
  
- CA4: Las cuentas corrientes NO deben recibir intereses.  
