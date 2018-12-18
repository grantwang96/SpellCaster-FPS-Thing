using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {

    void TakeDamage(int power);
    void TakeDamage(int power, Vector3 velocity);
    void AddForce(Vector3 velocity);
}
