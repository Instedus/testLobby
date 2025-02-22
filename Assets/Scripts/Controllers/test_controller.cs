using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_controller : MonoBehaviour
{
    [SerializeField] private float offset;

    private void Update()
    {
        //Vector3 dif = camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //diff = dif;
        //mousePos = Input.mousePosition;
        //float rotation = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
        //distance = rotation;
        //transform.rotation = Quaternion.Euler(0, 0, rotation+offset);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Создаем плоскость, которая проходит через позицию объекта и перпендикулярна оси Z
        Plane plane = new Plane(Vector3.forward, transform.position);

        // Проверяем, пересекает ли луч плоскость
        if (plane.Raycast(ray, out float hit))
        {
            // Получаем позицию мыши на плоскости
            Vector3 mousePos = ray.GetPoint(hit);

            // Вычисляем направление от объекта к позиции мыши
            Vector3 direction = (mousePos - transform.position).normalized;

            // Вычисляем угол между направлением и осью X
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Применяем вращение по оси Z
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + offset));
        }
    }
}
